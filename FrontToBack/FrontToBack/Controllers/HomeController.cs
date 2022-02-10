using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
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

        public IActionResult Index()
        {
            HttpContext.Session.SetString("session", "P320");

            Response.Cookies.Append("cookie", "Hello");
            Response.Cookies.Append("cookie", "Hello", new CookieOptions { Expires = System.DateTimeOffset.Now.AddHours(1) }); ;

            var sliderImages =  _dbContext.SliderImages.ToList();
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
    
    public async Task<IActionResult> Search(string searchedProduct)
        {
            if (string.IsNullOrEmpty(searchedProduct))
            {
                return NoContent();
            }


            var products = await _dbContext.Products
                .Where(x=> x.Name.ToLower().Contains(searchedProduct.ToLower()))
                .ToListAsync();


            //return Json(products);
       
        return PartialView("_SearchedProductPartial",products);
        
        
        }


        public async Task<IActionResult> Basket()
        {
            //var session = HttpContext.Session.GetString("session");
            //var cookie = Request.Cookies["cookie"];

            //return Content(session+ " - " + cookie);
            var basket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basket))
            {
                return Content("Empty");
            }

           // var basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
            var newBasket = new List<BasketViewModel>();
            //foreach (var basketViewModel in basketViewModels)
            //{
            //    var product = await _dbContext.Products.FindAsync(basketViewModel.ID);
            //    if (product == null)
            //        continue;

            //    newBasket.Add(new BasketViewModel
            //    {
            //        ID = product.Id,
            //        Name = product.Name,
            //        Price = product.Price,
            //        Image = product.Image,
            //        Count = basketViewModel.Count
            //    });
            //}

            basket = JsonConvert.SerializeObject(newBasket);
            Response.Cookies.Append("basket", basket);

            return Json(newBasket);
            //return View();
            
        }


        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null)
                return BadRequest();

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            List<BasketViewModel> basketViewModels;
            var existBasket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(existBasket))
            {
                basketViewModels = new List<BasketViewModel>();
            }
            else
            {
                basketViewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(existBasket);
            }

            var existBasketViewModel = basketViewModels.FirstOrDefault(x => x.ID == id);
            if (existBasketViewModel == null)
            {
                existBasketViewModel = new BasketViewModel
                {
                    ID = product.Id
                };
                basketViewModels.Add(existBasketViewModel);
            }
            else
            {
                existBasketViewModel.Count++;
            }

            var basket = JsonConvert.SerializeObject(basketViewModels);
            Response.Cookies.Append("basket", basket);

            return RedirectToAction(nameof(Index));

        }
    }
}
