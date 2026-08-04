using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Services
{
    /// <summary>Builds and verifies eSewa ePay v2 payment requests (sandbox by default —
    /// see Web.config EsewaFormUrl/EsewaStatusCheckUrl to switch to production).</summary>
    public static class EsewaService
    {
        public static string MerchantCode => ConfigurationManager.AppSettings["EsewaMerchantCode"];
        public static string SecretKey => ConfigurationManager.AppSettings["EsewaSecretKey"];
        public static string FormUrl => ConfigurationManager.AppSettings["EsewaFormUrl"];
        public static string StatusCheckUrl => ConfigurationManager.AppSettings["EsewaStatusCheckUrl"];

        private static string FormatAmount(decimal amount) => amount.ToString("0.00", CultureInfo.InvariantCulture);

        private static string Sign(string message)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>transactionUuid must be unique per payment attempt — callers should mint a
        /// fresh one for every redirect to eSewa, even when retrying the same order.</summary>
        public static ESewaPaymentModel BuildPaymentRequest(decimal amount, string transactionUuid, string successUrl, string failureUrl)
        {
            const string signedFieldNames = "total_amount,transaction_uuid,product_code";
            var totalAmount = FormatAmount(amount);
            var message = $"total_amount={totalAmount},transaction_uuid={transactionUuid},product_code={MerchantCode}";

            return new ESewaPaymentModel
            {
                Amount = FormatAmount(amount),
                TaxAmount = "0",
                TotalAmount = totalAmount,
                TransactionUuid = transactionUuid,
                ProductCode = MerchantCode,
                ProductServiceCharge = "0",
                ProductDeliveryCharge = "0",
                SuccessUrl = successUrl,
                FailureUrl = failureUrl,
                SignedFieldNames = signedFieldNames,
                Signature = Sign(message)
            };
        }

        /// <summary>Result of decoding the base64 `data` query param eSewa appends to success_url.</summary>
        public class CallbackResult
        {
            public string Status;
            public string TransactionUuid;
            public string TransactionCode;
            public decimal TotalAmount;
            public bool SignatureValid;
        }

        public static CallbackResult DecodeAndVerifyCallback(string base64Data)
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64Data));
            var obj = JObject.Parse(json);

            var signedFieldNames = (string)obj["signed_field_names"] ?? "";
            var providedSignature = (string)obj["signature"] ?? "";

            var fields = signedFieldNames.Split(',');
            var message = string.Join(",", Array.ConvertAll(fields, f => $"{f}={(string)obj[f]}"));
            var expectedSignature = Sign(message);

            return new CallbackResult
            {
                Status = (string)obj["status"],
                TransactionUuid = (string)obj["transaction_uuid"],
                TransactionCode = (string)obj["transaction_code"],
                TotalAmount = decimal.Parse((string)obj["total_amount"], NumberStyles.Any, CultureInfo.InvariantCulture),
                SignatureValid = string.Equals(providedSignature, expectedSignature, StringComparison.Ordinal)
            };
        }

        /// <summary>Server-to-server confirmation via eSewa's status check API — always call this
        /// before trusting a success redirect, since query-string params are client-supplied.</summary>
        public static string CheckStatus(string transactionUuid, decimal totalAmount)
        {
            var url = $"{StatusCheckUrl}?product_code={Uri.EscapeDataString(MerchantCode)}" +
                      $"&total_amount={Uri.EscapeDataString(FormatAmount(totalAmount))}" +
                      $"&transaction_uuid={Uri.EscapeDataString(transactionUuid)}";

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var body = reader.ReadToEnd();
                var obj = JObject.Parse(body);
                return (string)obj["status"];
            }
        }
    }
}
