using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 10;
            ViewBag.totalpage = Math.Ceiling((decimal)_dbContext.Products.Count() / take);
            ViewBag.currentpage = page;
            var products = await _dbContext.Products.Include(x => x.Category).Skip((page - 1) * take).Take(take).ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var product = await _dbContext.Products.Include(x=> x.Category).Where(x => x.Id == id).FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return View(product);

        }

        public IActionResult Create()
        {
            List<SelectListItem> categories = ( from i in _dbContext.Categorys.ToList()
                                               select new SelectListItem
                                               {
                                                   Text = i.Name,
                                                   Value = i.Id.ToString()
                                               }).ToList();
            ViewBag.dgr = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product products)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var isExistProduct = await _dbContext.Products.AnyAsync(x => x.Name.ToLower() == products.Name.ToLower());

            if (isExistProduct)
            {
                ModelState.AddModelError("Name", "Bu adda məhsul mövcuddur!");
                return View();
            }

            await _dbContext.Products.AddAsync(products);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var products = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (products == null)
                return NotFound();

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Product products)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var isExistProduct = await _dbContext.Products.FindAsync(products.Id);
            if (isExistProduct == null)
                return NotFound();

            bool isSameName = await _dbContext.Products.AnyAsync(x => x.Name.ToLower().Trim() == products.Name.ToLower().Trim());
            if (isSameName)
            {
                ModelState.AddModelError("Name", "Eyni adda category movcuddur");
                return View();
            }

            isExistProduct.Name = products.Name;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product products = await _dbContext.Products.FindAsync(id);
            if (products == null)
                return Json(new { status = 404 });

            _dbContext.Products.Remove(products);
            _dbContext.SaveChanges();
            return Json(new { status = 200 });
        }
    }
}
