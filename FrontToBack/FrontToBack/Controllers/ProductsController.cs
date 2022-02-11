using FrontToBack.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            ViewBag.ProductCount = _productsCount;

           // var products =await  _dbContext.Products.Include(x=>x.Category).Take(4).ToListAsync();

            return View(/*products*/);
        }


        public IActionResult Load(int skip)
        {
            #region MyRegion
            //var products = _dbContext.Products.Include(x => x.Category).Skip(4).Take(4).ToList();

            //           return Json(products);
            #endregion

            if (skip >= _productsCount)
            {
                return BadRequest();
            }

            var products =  _dbContext.Products.Include(x => x.Category).Skip(skip).Take(4).ToList();

            return PartialView("_ProductPartial", products);

        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var product =await _dbContext.Products.SingleOrDefaultAsync(x=> x.Id==id);
            if (product==null)
            {
                return NotFound();
            }

            return View(product);



        }
      
    }
}
