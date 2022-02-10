using FrontToBack.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _dbContext;

        public ProductViewComponent(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IViewComponentResult InvokeAsync(int take = 8)
        {
            var products =  _dbContext.Products.Include(x=>x.Category).Take(take).ToListAsync();

            return View(products);
        }

        
       

          
    
    }
}
