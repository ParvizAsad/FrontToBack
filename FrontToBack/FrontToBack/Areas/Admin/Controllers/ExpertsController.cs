using FrontToBack.Areas.Admin.Data;
using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExpertsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ExpertsController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var expert = await _dbContext.Experts.ToListAsync();
            return View(expert);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return BadRequest();

            var expert = await _dbContext.Experts.FindAsync(id);

            if (expert == null)
                return NotFound();

            return View(expert);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expert expert)
        {
            //if (!ModelState.IsValid)
            //   return View();

            if (!expert.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Yüklədiyiniz şəkil olmalıdır");
                return View();
            }

            if (!expert.Photo.IsAllowedSize(1))
            {
                ModelState.AddModelError("Photo", "Yüklədiyiniz şəkil 1Mb-dan az olmalıdır");
                return View();
            }

            var webRootPath = _environment.WebRootPath;
            var fileName = $"{Guid.NewGuid()}-{expert.Photo.FileName}";
            var path = Path.Combine(webRootPath, "img", fileName);

            var fileStream = new FileStream(path, FileMode.CreateNew);
            await expert.Photo.CopyToAsync(fileStream);

            expert.Image = fileName;
            await _dbContext.Experts.AddAsync(expert);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var expert = await _dbContext.Experts.FirstOrDefaultAsync(x => x.ID == id);
            if (expert == null)
                return NotFound();

            return View(expert);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, Expert expert)
        {
            if (id == null)
                return NotFound();

            if (id != expert.ID)
                return BadRequest();

            var existExpert = await _dbContext.Experts.FindAsync(id);
            if (existExpert == null)
                return NotFound();

           //if (!ModelState.IsValid)
           //   return View(existExpert);

            if (!expert.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Yuklediyiniz shekil olmalidir.");
                return View(existExpert);
            }

            if (!expert.Photo.IsAllowedSize(1))
            {
                ModelState.AddModelError("Photo", "1 mb-dan az olmalidir.");
                return View(existExpert);
            }

            var path = Path.Combine(Constants.ImageFolderPath, existExpert.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var fileName = await expert.Photo.GenerateFile(Constants.ImageFolderPath);
            existExpert.Image = fileName;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var expert = await _dbContext.Experts.FindAsync(id);
            if (expert == null)
                return NotFound();

            return View(expert);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteExpertImage(int? id)
        {
            if (id == null)
                return NotFound();

            var expert = await _dbContext.Experts.FindAsync(id);
            if (expert == null)
                return NotFound();
            _dbContext.Experts.Remove(expert);
            await _dbContext.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }

    }
}
