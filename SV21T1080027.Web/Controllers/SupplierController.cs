using Microsoft.AspNetCore.Mvc;

namespace SV21T1080027.Web.Controllers
{
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo nhà cung cấp";
            return View("Edit");
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa nhà cung cấp";
            return View();
        }
        public IActionResult Delete(int id = 0)
        {
            return View();
        }
    }
}
