using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;

namespace SV21T1080027.Web.Controllers
{
    public class ShipperController : Controller
    {
        const int PAGE_SIZE = 10;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            var rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue);
            int pageCount = (rowCount + PAGE_SIZE - 1) / PAGE_SIZE;

            ViewBag.Page = page;
            ViewBag.PageCount = pageCount;
            ViewBag.RowCount = rowCount;
            ViewBag.SearchValue = searchValue;

            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo đơn vị giao hàng";
            Shipper shipper = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", shipper);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa đơn vị giao hàng";
            Shipper? shipper = CommonDataService.GetShipper(id);
            if (shipper == null)
            {
                return RedirectToAction("Index");
            }
            return View(shipper);
        }

        public IActionResult Delete(int id = 0)
        {
            Shipper? shipper = CommonDataService.GetShipper(id);
            if (shipper == null)
            {
                return RedirectToAction("Index");
            }
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            ViewBag.AllowDelete = true;
            if (CommonDataService.IsUsedShipper(id))
            {
                ViewBag.AllowDelete = false;
            }
            return View(shipper);
        }

        [HttpPost]
        public IActionResult Save(Shipper shipper)
        {
            if (shipper.ShipperID == 0)
            {
                CommonDataService.AddShipper(shipper);
            } else
            {
                CommonDataService.UpdateShipper(shipper);
            }
            return RedirectToAction("Index");
        }
    }
}
