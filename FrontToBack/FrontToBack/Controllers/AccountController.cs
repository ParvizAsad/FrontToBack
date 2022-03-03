﻿using System.Threading.Tasks;
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
          //  await _signInManager.SignInAsync(user, false);

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = Url.Action(nameof(VerifyEmail), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());

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
            msg.Subject = "VerifyEmail";
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
            smtp.Send(msg);
            TempData["confirm"] = true;


            return RedirectToAction(nameof(Login));
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
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            if (existUser.IsDeleted)
            {
                ModelState.AddModelError("", "This user is deleted.");

                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser.UserName, loginModel.Password, loginModel.KeepMeSignedIn,  true);
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

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        #region MyRegion
        //public IActionResult ResetPassWord()
        //{
        //    return View();
        //}
        //public async Task<IActionResult> Verify(string email, string token)
        //{
        //    var user = await ._userManager.FindByEmailAsync(email);
        //    if (user == null) return BadRequest();

        //    await _userManager.ConfirmEmailAsync(user, token);
        //    await _signInManager.SignInAsync(user, true);
        //    TempData["confirmed"] = true;

        //    return RedirectToAction(nameof(Index), "Home");
        //}

        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel forgetPasswordVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(forgetPasswordVM.Email);
        //        if (user != null && await _userManager.IsEmailConfirmedAsync(user))
        //        {
        //            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //            var resetLink = Url.Action("ResetPassword", "Account", new { email = forgetPasswordVM.Email, token }, Request.Scheme, Request.Host.ToString());

        //            MailMessage message = new MailMessage();
        //            message.From = new MailAddress("codep320@gmail.com", "Fiorello");
        //            message.To.Add(user.Email);
        //            string body = string.Empty;
        //            using (StreamReader reader = new StreamReader("wwwroot/template/changepassword.html"))
        //            {
        //                body = await reader.ReadToEndAsync();
        //            }

        //            message.Body = body.Replace("{{link}}", resetLink);
        //            message.Subject = "Verify";
        //            message.IsBodyHtml = true;

        //            SmtpClient client = new SmtpClient();
        //            client.Host = "smtp.gmail.com";
        //            client.Port = 587;
        //            client.EnableSsl = true;
        //            client.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
        //            client.Send(message);
        //            TempData["confirm"] = true;

        //        }

        //        return View("ForgotPasswordConfirmation");
        //    }

        //    return View(forgetPasswordVM);
        //}

        //public async Task<IActionResult> ResetPassword(string token, string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    string mailtoken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    string link = Url.Action(nameof(Verify), "Account", new { email = user.Email, mailtoken }, Request.Scheme, Request.Host.ToString());

        //    MailMessage msg = new MailMessage();
        //    msg.From = new MailAddress("codep320@gmail.com", "Fiorello");
        //    msg.To.Add(user.Email);
        //    string body = string.Empty;
        //    using (StreamReader reader = new StreamReader("wwwroot/template/verifyemail.html"))
        //    {
        //        body = reader.ReadToEnd();
        //    }
        //    msg.Body = body.Replace("{{link}}", link);
        //    msg.Subject = "Verify";
        //    msg.IsBodyHtml = true;

        //    SmtpClient smtp = new SmtpClient();
        //    smtp.Host = "smtp.gmail.com";
        //    smtp.Port = 587;
        //    smtp.EnableSsl = true;
        //    smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
        //    smtp.Send(msg);
        //    TempData["confirm"] = true;
        //    if (token == null || email == null)
        //    {
        //        ModelState.AddModelError("", "Invalid Credentials");
        //    }
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        //{
        //    var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("Email", "Bu emailde user movcud deyl");
        //        return View();
        //    }

        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var result = await _userManager.ResetPasswordAsync(user, token,
        //        resetPasswordViewModel.Password);
        //    if (result.Succeeded)
        //    {
        //        return View("ResetPasswordComfirmation");
        //    }
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error.Description);
        //    }
        //    return View(resetPasswordViewModel);
        //}
        #endregion

        public IActionResult ForgotPassword()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme, Request.Host.ToString());

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("codep320@gmail.com", "Fiorello");
                    message.To.Add(user.Email);
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader("wwwroot/template/changepassword.html"))
                    {
                        body = await reader.ReadToEndAsync();
                    }

                    message.Body = body.Replace("{{link}}", resetLink);
                    message.Subject = "VerifyEmail";
                    message.IsBodyHtml = true;

                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
                    client.Send(message);
                    TempData["confirm"] = true;

                }

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        public IActionResult ResetPassword(string token, string email)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something get wrong");

                return View();
            }

            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Something get wrong. Try to get password reset link once more");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }

                return View("ResetPasswordConfirmation");
            }

            return View(model);
        }

        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest();
            }

            await _userManager.ConfirmEmailAsync(user, token);
            await _signInManager.SignInAsync(user, true);
            TempData["confirmed"] = true;

            return RedirectToAction(nameof(Index), "Home");
        }


    }

}

