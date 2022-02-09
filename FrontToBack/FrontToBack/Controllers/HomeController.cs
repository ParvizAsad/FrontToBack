using FrontToBack.DataAccessLayer;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var sliderImages = await _dbContext.SliderImages.ToListAsync();
            var slider = _dbContext.Sliders.SingleOrDefault();
            var categorys=_dbContext.Categorys.ToList();
            var products=_dbContext.Products.Include(x=>x.Category).ToList();
            var abouts = _dbContext.Abouts.SingleOrDefault();
            var aboutImages = _dbContext.AboutImages.SingleOrDefault();
            var aboutLists = _dbContext.AboutLists.ToList();
            var expertsTitles = _dbContext.ExpertsTitles.SingleOrDefault();
            var experts = _dbContext.Experts.ToList();
            var subscribes = _dbContext.Subscribes.SingleOrDefault();
            var blogTitles = _dbContext.BlogTitles.SingleOrDefault();
            var blogContent = _dbContext.BlogContents.ToList();
            var say = _dbContext.Says.ToList();
            var instagrams = _dbContext.Instagramss.ToList();



            return View(new HomeViewModel
            {
                SliderImages= sliderImages,
                Slider = slider,
                Categorys=categorys,    
                Products=products,
                Abouts= abouts,
                AboutLists= aboutLists, 
                AboutImages= aboutImages,
                ExpertsTitles=expertsTitles,   
                Experts = experts,
                Subscribes= subscribes,
                BlogTitles= blogTitles,
                BlogContents= blogContent,
                Says= say,
                Instagramss=instagrams
                
            });
        }
    }
}
