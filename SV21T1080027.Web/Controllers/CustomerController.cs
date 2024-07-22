using Microsoft.AspNetCore.Mvc;

namespace SV21T1080027.Web.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            var customers = BusinessLayers.CommonDataService.ListOfCustomers();
            return View();
        }

        public IActionResult Create()
        {
            return View("Edit");
        }

        public IActionResult Edit(int id = 0)
        {
            return View();
        }
        public IActionResult Delete(int id = 0)
        {
            return View();
        }
    }
}
