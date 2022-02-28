using FrontToBack.Areas.Admin.Data;
using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class SliderImagesController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        public SliderImagesController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var sliderImages = await _dbContext.SliderImages.ToListAsync();
            return View(sliderImages);
        }

        public IActionResult Create()
        {
            return View();
        }

        #region Single
        //public async Task<IActionResult> Create(SliderImage sliderImages)
        //{
        //    if(!ModelState.IsValid)
        //        return View();

        //    if(!sliderImages.Photo.IsImage())
        //    {
        //        ModelState.AddModelError("Photo", "Yükləməyiniz şəkil olmalıdır");
        //        return View();
        //    }

        //    if (!sliderImages.Photo.IsAllowedSize(1))
        //    {
        //        ModelState.AddModelError("Photo", "Yükləməyiniz şəkil 1Mb-dan az olmalıdır");
        //        return View();
        //    }

        //    var fileName = await sliderImages.Photo.GenerateFile(Constants.ImageFolderPath);

        //    sliderImages.Name = fileName;
        //    await _dbContext.SliderImages.AddAsync(sliderImages);
        //    await _dbContext.SaveChangesAsync();

        //    return RedirectToAction(nameof(Index));
        //}
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderImage sliderImage)
        {
            //if (!ModelState.IsValid)
            //    return View();

            var maxCount = 5;
            var imageCount = _dbContext.SliderImages.Count();
            var downloadImageCount = maxCount - imageCount;
            bool checkImageCount = sliderImage.Photos.Length > downloadImageCount;
            ViewBag.CheckCount = checkImageCount;
            if (checkImageCount)
            {
                if (downloadImageCount==0)
                {
                    ModelState.AddModelError("Photos", "Limiti tamamlandığı üçün shekil yükləyə bilməzsiniz.");
                    return View();
                }

                ModelState.AddModelError("Photos", $"Max {downloadImageCount} shekil yükləyə bilərsiniz.");
                return View();
            }
            
            foreach (var photo in sliderImage.Photos)
            {

                if (!photo.IsImage())
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} - Yuklediyiniz shekil olmalidir.");
                    return View();
                }

                if (!photo.IsAllowedSize(1))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} - shekil 1 mb-dan az olmalidir.");
                    return View();
                }

                var fileName = await photo.GenerateFile(Constants.ImageFolderPath);

                var newSliderImage = new SliderImage { Name = fileName };
                await _dbContext.SliderImages.AddAsync(newSliderImage);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var sliderImage = await _dbContext.SliderImages.FirstOrDefaultAsync(x => x.ID == id);
            if (sliderImage == null)
                return NotFound();

            return View(sliderImage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, SliderImage sliderImage)
        {
            if (id == null)
                return NotFound();

            if (id != sliderImage.ID)
                return BadRequest();

            var existSliderImage = await _dbContext.SliderImages.FindAsync(id);
            if (existSliderImage == null)
                return NotFound();

            //if (!ModelState.IsValid)
            //    return View(existSliderImage);

            if (!sliderImage.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "Yuklediyiniz shekil olmalidir.");
                return View(existSliderImage);
            }

            if (!sliderImage.Photo.IsAllowedSize(1))
            {
                ModelState.AddModelError("Photo", "1 mb-dan az olmalidir.");
                return View(existSliderImage);
            }

            var path = Path.Combine(Constants.ImageFolderPath, existSliderImage.Name);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var fileName = await sliderImage.Photo.GenerateFile(Constants.ImageFolderPath);
            existSliderImage.Name = fileName;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var sliderImage = await _dbContext.SliderImages.FindAsync(id);
            if (sliderImage == null)
                return NotFound();

            return View(sliderImage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteSliderImage(int? id)
        {
            if (id == null)
                return NotFound();

            var sliderImage = await _dbContext.SliderImages.FindAsync(id);
            if (sliderImage == null)
                return NotFound();
            _dbContext.SliderImages.Remove(sliderImage);
            await _dbContext.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }

    }
}
