using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 10;
            ViewBag.totalpage = Math.Ceiling((decimal)_dbContext.Products.Count() / take);
            ViewBag.currentpage = page;
            var blogs = await _dbContext.BlogContents.Skip((page - 1) * take).Take(take).ToListAsync();
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
            var isExistBlog = await _dbContext.BlogContents.AnyAsync(x => x.Title.ToLower() == blogContents.Title.ToLower());

            if (isExistBlog)
            {
                ModelState.AddModelError("Title", "Bu title-da blog mövcuddur!");
                return View();
            }

            await _dbContext.BlogContents.AddAsync(blogContents);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {
            var blog = await _dbContext.BlogContents.FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BlogContent blogContent)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var existBlog = await _dbContext.BlogContents.FindAsync(blogContent.Id);
            if (existBlog == null)
                return NotFound();

            bool isSameTitle = await _dbContext.BlogContents.AnyAsync(x => x.Title.ToLower().Trim() == blogContent.Title.ToLower().Trim());
            if (isSameTitle)
            {
                ModelState.AddModelError("Title", "Eyni adda blog movcuddur");
                return View();
            }

            existBlog.Title = blogContent.Title;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            BlogContent blogContents = await _dbContext.BlogContents.FindAsync(id);
            if (blogContents == null)
                return Json(new { status = 404 });

            _dbContext.BlogContents.Remove(blogContents);
            _dbContext.SaveChanges();
            return Json(new { status = 200 });
        }


    }
}
