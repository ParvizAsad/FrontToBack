using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public BlogController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
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

            if (!blogContents.Photo.ContentType.Contains("image"))
            {
                ModelState.AddModelError("Photo", "Yükləməyiniz şəkil olmalıdır");
                return View();
            }

            if (blogContents.Photo.Length > 1024 * 1000)
            {
                ModelState.AddModelError("Photo", "Yükləməyiniz şəkil 1Mb-dan az olmalıdır");
                return View();
            }

            var webRootPath = _environment.WebRootPath;
            var fileName = $"{Guid.NewGuid()}-{blogContents.Photo.FileName}";
            var path = Path.Combine(webRootPath, "img", fileName);

            var fileStream = new FileStream(path, FileMode.CreateNew);
            await blogContents.Photo.CopyToAsync(fileStream);

            blogContents.Image = fileName;
            await _dbContext.BlogContents.AddAsync(blogContents);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var blog = await _dbContext.BlogContents.FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, BlogContent blogContent)
        {
            if (id == null)
                return NotFound();

            if (id != blogContent.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View();
            }

            var existBlog = await _dbContext.BlogContents.FindAsync(blogContent.Id);
            if (existBlog == null)
                return NotFound();

            bool isExist = await _dbContext.BlogContents.AnyAsync(x => x.Title.ToLower().Trim() == blogContent.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "Eyni adda blog movcuddur");
                return View();
            }

            existBlog.Title = blogContent.Title;
            existBlog.date = blogContent.date;
            existBlog.Content = blogContent.Content;
            existBlog.Image=blogContent.Image;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var blogContents = await _dbContext.BlogContents.FindAsync(id);
            if (blogContents == null)
                return NotFound();
            
            return View(blogContents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteBlog(int? id)
        {
            if (id == null)
                return NotFound();

            var blogContents = await _dbContext.BlogContents.FindAsync(id);
            if (blogContents == null)
                return NotFound();

            _dbContext.BlogContents.Remove(blogContents);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
