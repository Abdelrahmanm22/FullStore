using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Store.Contexts;
using Store.Models;
using static System.Collections.Specialized.BitVector32;

namespace Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext dbContext;

        public HomeController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            //The newest products section
            var Products = dbContext.Products.OrderByDescending(p=>p.Id).Take(4).ToList();
            return View(Products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
