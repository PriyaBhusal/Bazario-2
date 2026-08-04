using System;
using System.Globalization;

namespace OnlineRetailStore.Mvc.Services
{
    /// <summary>Formats amounts as Nepali Rupees (NPR) regardless of server culture,
    /// instead of relying on culture-dependent "{0:C}" formatting.</summary>
    public static class MoneyHelper
    {
        public static string Format(decimal amount)
        {
            return "Rs. " + amount.ToString("N2", CultureInfo.InvariantCulture);
        }

        public static string Format(object amount)
        {
            return Format(Convert.ToDecimal(amount));
        }
    }
}
