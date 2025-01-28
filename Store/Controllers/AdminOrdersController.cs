using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Contexts;

namespace Store.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly AppDbContext dbContext;

        
        public AdminOrdersController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {

            var orders = dbContext.Orders.Include(o=>o.Client).Include(o=>o.Itmes).OrderByDescending(o=>o.Id).ToList();
            ViewBag.Orders = orders;
            return View();
        }
        public IActionResult Details(int id)
        {
            var order = dbContext.Orders.Include(o => o.Client).Include(o => o.Itmes)
                .ThenInclude(oi => oi.Product).FirstOrDefault(o => o.Id == id);


            if (order == null)
            {
                return RedirectToAction("Index");
            }


            ViewBag.NumOrders = dbContext.Orders.Where(o => o.ClientId == order.ClientId).Count();

            return View(order);
        }
        public IActionResult Edit(int id, string? payment_status, string? order_status)
        {
            var order = dbContext.Orders.Find(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }


            if (payment_status == null && order_status == null)
            {
                return RedirectToAction("Details", new { id });
            }

            if (payment_status != null)
            {
                order.PaymentStatus = payment_status;
            }

            if (order_status != null)
            {
                order.OrderStatus = order_status;
            }

            dbContext.SaveChanges();


            return RedirectToAction("Details", new { id });
        }
    }
}
