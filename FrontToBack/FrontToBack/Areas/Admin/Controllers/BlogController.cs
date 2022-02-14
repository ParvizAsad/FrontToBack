using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;

        public BlogController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _dbContext.BlogContents.ToListAsync();

            return View(blogs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var blogs = await _dbContext.BlogContents.FindAsync(id);

            if (blogs == null)
                return NotFound();

            return View(blogs);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogContent blogContents)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var isExistCategory = await _dbContext.BlogContents.AnyAsync(x => x.Title.ToLower() == blogContents.Title.ToLower());

            if (isExistCategory)
            {
                ModelState.AddModelError("Title", "Bu title-da blog mövcuddur!");
                return View();
            }

            await _dbContext.BlogContents.AddAsync(blogContents);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
