using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public async Task<IActionResult> Index()
        {
            HttpContext.Session.SetString("session", "P320");

            Response.Cookies.Append("cookie", "Hello");
            Response.Cookies.Append("cookie", "Hello", new CookieOptions { Expires = System.DateTimeOffset.Now.AddHours(1) }); ;

            var sliderImages = await _dbContext.SliderImages.ToListAsync();
            var slider = await _dbContext.Sliders.SingleOrDefaultAsync();
            var categorys= await _dbContext.Categorys.ToListAsync();
        // var products= await _dbContext.Products.Include(x=>x.Category).ToListAsync();
            var abouts = await _dbContext.Abouts.SingleOrDefaultAsync();
            var aboutImages = await _dbContext.AboutImages.SingleOrDefaultAsync();
            var aboutLists = await _dbContext.AboutLists.ToListAsync();
            var expertsTitles = await _dbContext.ExpertsTitles.SingleOrDefaultAsync();
            var experts = await _dbContext.Experts.ToListAsync();
            var subscribes = await _dbContext.Subscribes.SingleOrDefaultAsync();
            var blogTitles = await _dbContext.BlogTitles.SingleOrDefaultAsync();
            var blogContent = await _dbContext.BlogContents.ToListAsync();
            var say = await _dbContext.Says.ToListAsync();
            var instagrams = await _dbContext.Instagramss.ToListAsync();



            return View(new HomeViewModel
            {
                SliderImages=  sliderImages,
                Slider =  slider,
                Categorys=  categorys,    
           // Products=products,
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

            var basket = Request.Cookies["basket"];
            if (string.IsNullOrEmpty(basket))
            {
                return Content("Empty");
            }

            var basketViewModels= JsonConvert.DeserializeObject<List<BasketViewModel>>(basket); 
         var newBasket=new List<BasketViewModel>();
            foreach (var basketViewModel in basketViewModels)
            {
                var product = await _dbContext.Products.FindAsync(basketViewModel.ID);

                if (product==null)
                {
                    continue;
                }


                newBasket.Add(new BasketViewModel
                {
                    ID=product.Id,  
                    Name=product.Name,  
                    Price=product.Price,
                    Count=basketViewModel.Count,
                    Image=product.Image,
                });

            }
             basket = JsonConvert.SerializeObject(basketViewModels);
            Response.Cookies.Append("basket", basket);

            // return Json(newBasket);
            return View(newBasket);

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

            var existBasketViewModel=basketViewModels.FirstOrDefault(x=>x.ID==id);
            if (existBasketViewModel==null)
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



        public IActionResult MinusCount(int? id)
        {
            var basket = Request.Cookies["basket"];
           
            if (id == null)
                return BadRequest();
            
            if (string.IsNullOrEmpty(basket))
                return BadRequest();
            
            var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
            
            foreach (var item in products)
            {
                if (item.ID == id)
                {
                    item.Count--;
                    if (item.Count == 0)
                        products = products.Where(x => x.ID != id).ToList();
                }
            }
            
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products));
            // return View("Basket");
            return RedirectToAction(nameof(Basket));
        }

        public IActionResult PlusCount(int? id)
        {
            var basket = Request.Cookies["basket"];
           
            if (id == null)
                return NotFound();
           
           
            if (string.IsNullOrEmpty(basket))
                return NotFound();
            
            var products = JsonConvert.DeserializeObject<List<BasketViewModel>>(basket);
           
            foreach (var item in products)
            {
                if (item.ID == id)
                {
                    item.Count++;
                }
            }
            
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products));
            //Page.Response.Redirect(Page.Request.Url.ToString(), False);
            return RedirectToAction(nameof(Basket));

        }


    }
}
