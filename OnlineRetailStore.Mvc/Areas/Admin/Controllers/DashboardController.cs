using System;
using System.Linq;
using System.Web.Mvc;
using OnlineRetailStore.Mvc.Areas.Admin.Models;

namespace OnlineRetailStore.Mvc.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            var nonCancelled = Db.Orders.Where(o => o.Status != "Cancelled");

            var model = new AdminDashboardVm
            {
                TotalProducts = Db.Products.Count(),
                TotalVendors = Db.Users.Count(u => u.Role == "Vendor"),
                TotalCustomers = Db.Users.Count(u => u.Role == "User"),
                TotalOrders = Db.Orders.Count(),
                TotalSales = nonCancelled.Sum(o => (decimal?)o.Total) ?? 0,
                LowStockCount = Db.Products.Count(p => p.Stock <= 5)
            };

            // Sales over the last 14 days
            var since = DateTime.Today.AddDays(-13);
            var dailySales = nonCancelled
                .Where(o => o.CreatedAt >= since)
                .ToList()
                .GroupBy(o => o.CreatedAt.Date)
                .ToDictionary(g => g.Key, g => g.Sum(o => o.Total));

            for (var day = since; day <= DateTime.Today; day = day.AddDays(1))
            {
                model.SalesLabels.Add(day.ToString("MMM d"));
                model.SalesData.Add(dailySales.TryGetValue(day, out var total) ? total : 0);
            }

            // Orders by status
            var byStatus = Db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            foreach (var s in byStatus)
            {
                model.StatusLabels.Add(s.Status);
                model.StatusData.Add(s.Count);
            }

            // Top 5 products by quantity sold
            var topProducts = Db.OrderItems
                .GroupBy(oi => oi.ProductName)
                .Select(g => new { Name = g.Key, Qty = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(x => x.Qty)
                .Take(5)
                .ToList();

            foreach (var p in topProducts)
            {
                model.TopProductLabels.Add(p.Name);
                model.TopProductData.Add(p.Qty);
            }

            return View(model);
        }
    }
}
