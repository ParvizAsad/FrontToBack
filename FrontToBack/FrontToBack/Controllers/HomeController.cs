using FrontToBack.DataAccessLayer;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var sliderImages = _dbContext.SliderImages.ToList();
            var slider = _dbContext.Sliders.SingleOrDefault();
            var categorys=_dbContext.Categorys.ToList();
            var products=_dbContext.Products.Include(x=>x.Category).ToList();
            var abouts = _dbContext.Abouts.SingleOrDefault();
            var aboutImages = _dbContext.AboutImages.SingleOrDefault();
            var aboutLists = _dbContext.AboutLists.ToList();

            return View(new HomeViewModel
            {
                SliderImages= sliderImages,
                Slider = slider,
                Categorys=categorys,    
                Products=products,
                Abouts= abouts,
                AboutLists= aboutLists, 
                AboutImages= aboutImages

            });
        }
    }
}
