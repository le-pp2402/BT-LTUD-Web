using Microsoft.AspNetCore.Mvc;

namespace SV21T1080027.Web.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm nhân viên";
            return View("Edit");
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa nhân viên";
            return View();
        }
        public IActionResult Delete(int id = 0)
        {
            return View();
        }
    }
}
