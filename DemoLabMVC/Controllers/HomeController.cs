using DemoLabModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LabDemoWebASPMVC.Controllers
{
    /// <summary>
    ///   <br />
    /// </summary>
    /// <Modified>
    /// Name Date Comments
    /// khanhnn 5/24/2021 created
    /// </Modified>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        /// <summary>The configuration</summary>
        /// Controler hiển thị mà hình login, thực hiện login, logout
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>Indexes this instance.</summary>
        /// <returns>
        ///   Hiển thị màn hình login.
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public IActionResult Index()
        {
            if (TempData["status"] != null)
            {
                ViewBag.Message = TempData["status"].ToString();
                TempData.Remove("status");
            }
            return View();
        }

        /// <summary>Logins the specified admin.</summary>
        /// <param name="admin">The admin.</param>
        /// <returns>
        ///   Thực hiện login và chuyển đến màn hình quản lí nhân viên
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Admin admin)
        {
            string id = _configuration.GetValue<string>("Login:ID");
            string pass = _configuration.GetValue<string>("Login:Password");
            if (id != null && id.CompareTo(admin.Id) == 0 && pass != null && pass.CompareTo(admin.Password) == 0)
            {
                TempData.Remove("status");
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, admin.Id)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var props = new AuthenticationProperties();
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props).Wait();
                return RedirectToAction("Index", "Users");
            }
            TempData["status"] = "LoginID hoặc Password bị sai";
            return RedirectToAction("Index");
        }

        /// <summary>Logouts this instance.</summary>
        /// <returns>
        ///   Thực hiện logout
        /// </returns>
        /// <Modified>
        /// Name Date Comments
        /// khanhnn 5/24/2021 created
        /// </Modified>
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }



    }
}
