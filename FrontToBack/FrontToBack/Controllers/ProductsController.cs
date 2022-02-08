using FrontToBack.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FrontToBack.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly int _productsCount;

     

        public ProductsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _productsCount = _dbContext.Products.Count();
        }

        public IActionResult Index()
        {
            ViewBag.ProductCount = _productsCount;

            var products = _dbContext.Products.Include(x=>x.Category).Take(4).ToList();

            return View(products);
        }


        public IActionResult Load(int Skip)
        {
            #region MyRegion
            //var products = _dbContext.Products.Include(x => x.Category).Skip(4).Take(4).ToList();

            //           return Json(products);
            #endregion

            if (Skip >= _productsCount)
            {
                return BadRequest();
            }

            var products = _dbContext.Products.Include(x => x.Category).Skip(Skip).Take(4).ToList();

            return PartialView("_ProductPartial", products);

        }
    }
}
