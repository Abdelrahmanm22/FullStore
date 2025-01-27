using Microsoft.AspNetCore.Mvc;
using Store.Contexts;
using Store.Models;

namespace Store.Controllers
{
    public class StoreController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly int pageSize = 8;

        public StoreController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            IQueryable<Product> query = dbContext.Products;

            // search functionality
            if (search != null && search.Length > 0)
            {
                query = query.Where(p => p.Name.Contains(search));
            }


            // filter functionality
            if (brand != null && brand.Length > 0)
            {
                query = query.Where(p => p.Brand.Contains(brand));
            }

            if (category != null && category.Length > 0)
            {
                query = query.Where(p => p.Category.Contains(category));
            }

            // sort functionality
            if (sort == "price_asc")
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (sort == "price_desc")
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                // newest products first
                query = query.OrderByDescending(p => p.Id);
            }



            // pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);


            var products = query.ToList();

            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            var storeSearchViewModel = new StoreSearchViewModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort
            };

            return View(storeSearchViewModel);
        }

        public IActionResult Details(int? id)
        {
            if (id is null) return BadRequest();
            var Product = dbContext.Products.Find(id);
            if (Product is null) return NotFound();
            return View(Product);
        }
    }
}
