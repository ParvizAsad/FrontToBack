using System.Threading.Tasks;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
                return View();

            var existUser = await _userManager.FindByNameAsync(registerModel.Username);
            if (existUser != null)
            {
                ModelState.AddModelError("Username", "Bu adda user var.");
                return View();
            }

            var user = new User
            {
                Email = registerModel.Email,
                UserName = registerModel.Username,
                FullName = registerModel.FullName
            };

            var result = await _userManager.CreateAsync(user, registerModel.PassWord);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }
            await _signInManager.SignInAsync(user, false);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = Url.Action(nameof(Verify), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("codep320@gmail.com", "Fiorello");
            msg.To.Add(user.Email);
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/template/verifyemail.html"))
            {
                body = reader.ReadToEnd();
            }
            msg.Body = body.Replace("{{link}}", link);
            body = body.Replace("{{name}}", $"Welcome, {user.UserName.ToUpper()}");
            msg.Subject = "Verify";
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
            smtp.Send(msg);
            TempData["confirm"] = true;


            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Verify(string email, string token)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest();
            await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, true);
            TempData["confirmed"] = true;

            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return View();

            var existUser = await _userManager.FindByNameAsync(loginModel.Username);
            if (existUser == null)
            {
                ModelState.AddModelError("UserNme", "Invalid credentials");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser, loginModel.Password, false, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "This user is locked out.");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult ResetPassWord()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel forgetPassword)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(forgetPassword.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Username incorrect");
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);


            var link = Url.Action(nameof(Verify), "Account", new { email = user.Email, token }, Request.Scheme);
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("codep320@gmail.com", "Fiorello");
                mail.To.Add(user.Email);
                mail.Subject = "Reset Password";
                mail.Body = $"<a href={link}>Go to Reset Password</a>";
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);

                }
            }

                return View(nameof(Index),"Home");
            }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null) NotFound();

            ForgetPasswordViewModel forgetPassword = new ForgetPasswordViewModel
            {
                Token = token,
                User = user
            };
            return View(forgetPassword);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ForgetPasswordViewModel model)
        {
            User user = await _userManager.FindByEmailAsync(model.User.Email);
            if (user == null) NotFound();

            ForgetPasswordViewModel forgetPassword = new ForgetPasswordViewModel
            {
                Token = model.Token,
                User = user
            };
            //if (!ModelState.IsValid) return View(forgetPassword);


            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);

            }
            return RedirectToAction("Index", "Home");
        }

    }
}
