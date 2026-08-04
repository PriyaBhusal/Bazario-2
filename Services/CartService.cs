using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineRetailStore.Mvc.Services
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    /// <summary>Session-backed shopping cart - the MVC equivalent of the legacy app's
    /// Session["Cart"] DataTable (see CartHelper.cs in the Web Forms project).</summary>
    public static class CartService
    {
        private const string SessionKey = "Cart";

        public static List<CartItem> GetCart(HttpSessionStateBase session)
        {
            return session[SessionKey] as List<CartItem> ?? new List<CartItem>();
        }

        private static void SaveCart(HttpSessionStateBase session, List<CartItem> cart)
        {
            session[SessionKey] = cart;
        }

        public static void AddToCart(HttpSessionStateBase session, int productId, string name, decimal price)
        {
            var cart = GetCart(session);
            var existing = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existing != null) existing.Quantity++;
            else cart.Add(new CartItem { ProductId = productId, Name = name, Quantity = 1, UnitPrice = price });
            SaveCart(session, cart);
        }

        public static void Increment(HttpSessionStateBase session, int productId)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null) return;
            item.Quantity++;
            SaveCart(session, cart);
        }

        public static void Decrement(HttpSessionStateBase session, int productId)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item == null) return;
            item.Quantity--;
            if (item.Quantity <= 0) cart.Remove(item);
            SaveCart(session, cart);
        }

        public static void Remove(HttpSessionStateBase session, int productId)
        {
            var cart = GetCart(session);
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null) cart.Remove(item);
            SaveCart(session, cart);
        }

        public static void Clear(HttpSessionStateBase session)
        {
            session[SessionKey] = null;
        }

        public static int ItemCount(HttpSessionStateBase session)
        {
            return GetCart(session).Sum(c => c.Quantity);
        }

        public static decimal Total(HttpSessionStateBase session)
        {
            return GetCart(session).Sum(c => c.LineTotal);
        }

        /// <summary>Mirrors the legacy app's Session["UserId"], set at login/registration.</summary>
        public static int GetCurrentUserId(HttpSessionStateBase session)
        {
            return session["UserId"] != null ? System.Convert.ToInt32(session["UserId"]) : 0;
        }
    }
}
