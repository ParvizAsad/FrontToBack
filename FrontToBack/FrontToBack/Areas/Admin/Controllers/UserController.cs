using FrontToBack.Areas.Admin.ViewModels;
using FrontToBack.DataAccessLayer;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager = null, AppDbContext dbContext = null)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int take = 10;
            ViewBag.currentpage = page;
            var users = await _userManager.Users.Skip((page - 1) * take).Take(take).ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            var UserRoles = await _dbContext.UserRoles.ToListAsync();
            //if (!User.IsInRole("Admin"))
            //{

            //}    
            return View(new UserViewModel()
            {
                Users = users,
                Roles = roles,
                UserRoles = UserRoles
            });
        }

        public async Task<IActionResult> ChangeRol(string id)
        {
            if (id == null)
                return BadRequest();

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();

            return View();
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            if (id == null)
                return BadRequest();

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordViewModel changePasswordViewModel)
        {
            if (id == null)
                return NotFound();

            //if (!ModelState.IsValid)
            //    return View(changePasswordViewModel);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var oldPassword = await _userManager.CheckPasswordAsync(user, changePasswordViewModel.OldPassword);
            if (!oldPassword)
                return BadRequest();

            await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.Password);

            return View(nameof(Index));
        }

        public async Task<IActionResult> IsActive()
        {
            return View();
        }

        public async Task<IActionResult> IsDeactive()
        {
            return View();
        }


    }
}
