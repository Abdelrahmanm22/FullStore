using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Contexts;
using Store.Helpers;
using Store.Models;

namespace Store.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}")]
    public class ProductController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper _mapper;
        private readonly int pageSize = 5;

        public ProductController(AppDbContext dbContext,IMapper mapper)
        {
            this.dbContext = dbContext;
            _mapper = mapper;
        }
        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = dbContext.Products;

            // search functionality
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search));
            }

            // sort functionality
            string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!validOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }

            if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }
            else if (column == "Brand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            //query = query.OrderByDescending(p => p.Id);

            //pagination functionality
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductViewModel productVM)
        {
            
            if (ModelState.IsValid) {
                if (productVM.Image == null)
                    productVM.ImageFileName = null;
                else
                    productVM.ImageFileName = DocumentSettings.UplaodFile(productVM.Image, "Products");
                var MappedProduct = _mapper.Map<ProductViewModel, Product>(productVM);
                dbContext.Products.Add(MappedProduct);
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(productVM);
        }
        public IActionResult Edit(int? id)
        {
            if (id is null) return BadRequest();

            var Employee = dbContext.Products.Find(id);

            if (Employee is null) return NotFound();

            var MappedProduct = _mapper.Map<Product,ProductViewModel>(Employee);
            return View(MappedProduct);
        }

        [HttpPost]
        public IActionResult Edit(ProductViewModel productVM)
        {
            if (ModelState.IsValid) {
                try
                {
                    if (productVM.Image is not null)
                        productVM.ImageFileName = DocumentSettings.UplaodFile(productVM.Image,"Products");
                    var MappedProduct = _mapper.Map<ProductViewModel,Product>(productVM);
                    dbContext.Products.Update(MappedProduct);
                    dbContext.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) { 
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(productVM);
        }

        public IActionResult Delete(int? id) {
            if (id is null) return BadRequest();
            var Employee = dbContext.Products.Find(id);
            if (Employee is null) return NotFound();

            try
            {
                dbContext.Products.Remove(Employee);
                int result = dbContext.SaveChanges();
                if (result > 0 && Employee.ImageFileName is not null)
                {
                    DocumentSettings.DeleteFile(Employee.ImageFileName, "Products");
                }
                
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty,ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
