using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{ [Area("Admin")]
    public class ExpertsTitleController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ExpertsTitleController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var expertTitle =await _dbContext.ExpertsTitles.ToListAsync();

            return View(expertTitle);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
                return NotFound();

            var expertTitle = await _dbContext.ExpertsTitles.FirstOrDefaultAsync(x => x.ID == id);

            if (expertTitle == null)
                return NotFound();

            return View(expertTitle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ExpertsTitle expertTitle)
        {
            if (id == null)
                return NotFound();

            if (id != expertTitle.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View();

            var existExpertTitle = await _dbContext.ExpertsTitles.FindAsync(id);
            if (existExpertTitle == null)
                return NotFound();

            var isExist = await _dbContext.ExpertsTitles
                .AnyAsync(x => x.Title.ToLower().Trim() == expertTitle.Title.ToLower().Trim() && x.ID != id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "Eyni adda category movcuddur");
                return View();
            }

            existExpertTitle.Title = expertTitle.Title;
            existExpertTitle.SubTitle = expertTitle.SubTitle;

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
