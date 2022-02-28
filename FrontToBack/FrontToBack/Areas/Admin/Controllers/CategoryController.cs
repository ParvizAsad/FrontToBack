using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
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

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _dbContext.Categorys.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            if (id == null)
                return NotFound();

            if (id != category.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View();

            var existCategory = await _dbContext.Categorys.FindAsync(id);
            if (existCategory == null)
                return NotFound();

            var isExist = await _dbContext.Categorys
                .AnyAsync(x => x.Name.ToLower().Trim() == category.Name.ToLower().Trim() && x.Id!=id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "Eyni adda category movcuddur");
                return View();
            }

            existCategory.Name = category.Name;
        existCategory.Description = category.Description;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _dbContext.Categorys.FindAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _dbContext.Categorys.FindAsync(id);
            if (category == null)
                return NotFound();

            _dbContext.Categorys.Remove(category);
            await _dbContext.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }
    }
}
