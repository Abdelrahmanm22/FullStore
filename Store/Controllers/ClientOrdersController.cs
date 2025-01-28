using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Contexts;
using Store.Models;

namespace Store.Controllers
{
    [Authorize(Roles = "client")]
    [Route("/Client/Orders/{action=Index}/{id?}")]
    public class ClientOrdersController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly UserManager<User> userManager;
        private readonly int pageSize = 5;

        public ClientOrdersController(AppDbContext dbContext, UserManager<User> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }
        

        public async Task<IActionResult> Index(int pageIndex)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<Order> query = dbContext.Orders
                .Include(o => o.Itmes).OrderByDescending(o => o.Id)
                .Where(o => o.ClientId == currentUser.Id);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }


            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);

            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var orders = query.ToList();

            ViewBag.Orders = orders;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View();
        }


        public async Task<IActionResult> Details(int id)
        {
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = dbContext.Orders.Include(o => o.Itmes)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.ClientId == currentUser.Id).FirstOrDefault(o => o.Id == id);


            if (order == null)
            {
                return RedirectToAction("Index");
            }


            return View(order);
        }
    }
}
