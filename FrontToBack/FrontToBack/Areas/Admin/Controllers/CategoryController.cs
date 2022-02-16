using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CategoryController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int page =1)
        {
            int take = 10;
            ViewBag.totalpage = Math.Ceiling((decimal)_dbContext.Products.Count() / take);
            ViewBag.currentpage = page;
            var categories = await _dbContext.Categorys.Skip((page - 1) * take).Take(take).ToListAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id==null)
                return BadRequest();
            
            var category = await _dbContext.Categorys.FindAsync(id);
            
            if (category==null)
                return NotFound();  

            return View(category);  

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
               return View();
            }
            var isExistCategory = await _dbContext.Categorys.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower());

            if (isExistCategory)
            {
                ModelState.AddModelError("Name", "Bu adda kateqorya mövcuddur!");
                return View();
            }
            
            await _dbContext.Categorys.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _dbContext.Categorys.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var existCategory = await _dbContext.Categorys.FindAsync(category.Id);
            if (existCategory == null)
                return NotFound();

            bool isSameName = await _dbContext.Categorys.AnyAsync(x => x.Name.ToLower().Trim() == category.Name.ToLower().Trim());
            if (isSameName)
            {
                ModelState.AddModelError("Name", "Eyni adda category movcuddur");
                return View();
            }

            existCategory.Name = category.Name;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Category categorys = await _dbContext.Categorys.FindAsync(id);
            if (categorys == null)
                return Json(new { status = 404 });

            _dbContext.Categorys.Remove(categorys);
            _dbContext.SaveChanges();
            return Json(new { status = 200 });
        }

    }
}
