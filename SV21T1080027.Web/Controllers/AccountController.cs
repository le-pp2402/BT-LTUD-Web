using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web;
using System.Text.RegularExpressions;

namespace SV21T1080027.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.UserName = username;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                return View();
            }

            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại!");
                return View();
            }

            //Đăng nhập thành công, tạo dữ liệu để lưu thông tin đăng nhập
            ViewBag.Message = "";
            var userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList()
            };
            //Thiết lập phiên đăng nhập cho tài khoản
            await HttpContext.SignInAsync(userData.CreatePrincipal());
            //Redirec về trang chủ sau khi đăng nhập
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        const string FAIL_COUNT = "fail_count";
        const string STATUS = "change_password_status";

        [HttpPost]
        public IActionResult ChangePassword(String oldPassword, String newPassword, String confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ApplicationContext.SetSessionData(STATUS, "Nhập lại mật khẩu sai, hãy cẩn thận");
                return RedirectToAction("ChangePassword");
            }

            var fail = 0;
            if (ApplicationContext.GetSessionData<String>(FAIL_COUNT) == null)
            {
                fail = 0;
            }

            if (UserAccountService.Authorize(User.GetUserData().Email, oldPassword) == null)
            {
                fail++;
                ApplicationContext.SetSessionData(STATUS, "Đổi mật khẩu thất bại, bạn còn " + Convert.ToString(3 - fail) + "lần thử");
            } else
            {
                UserAccountService.ChangePassword(User.GetUserData().UserName, oldPassword, newPassword);
                fail = 0;
                ApplicationContext.SetSessionData(STATUS, "Đổi mật khẩu thành công");
            }

            ApplicationContext.SetSessionData(FAIL_COUNT, Convert.ToString(fail));

            if (fail >= 3)
            {
                return RedirectToAction("Logout");
            } else
            {
                return RedirectToAction("ChangePassword"); 
            }
        }
    }
}
