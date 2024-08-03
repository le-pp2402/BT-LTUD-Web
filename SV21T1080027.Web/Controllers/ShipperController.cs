using Microsoft.AspNetCore.Mvc;

namespace SV21T1080027.Web.Controllers
{
    public class ShipperController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo đơn vị giao hàng";
            return View("Edit");
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa đơn vị giao hàng";
            return View();
        }

        public IActionResult Delete(int id = 0)
        {
            return View();
        }
    }
}
