using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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


        public async Task<IActionResult> Index()
        {
            var categories = await _dbContext.Categorys.ToListAsync();

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

    }
}
