using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web.Models;

namespace SV21T1080027.Web.Controllers
{
    [Authorize]
    public class ShipperController : Controller
    {
        const int PAGE_SIZE = 10;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            var rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, page, PAGE_SIZE, searchValue);
            ShipperSearchResult shipperSearchResult = new ShipperSearchResult 
            {
                Page = page,
                RowCount = rowCount,
                SearchValue = searchValue,
                PageSize = PAGE_SIZE,
                Data = data
            };

            return View(shipperSearchResult);
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
