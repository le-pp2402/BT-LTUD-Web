using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web.Models;

namespace SV21T1080027.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        const int PAGE_SIZE = 20;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            var rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, page, PAGE_SIZE, searchValue);
            SupplierSearchResult supplierSearchResult = new SupplierSearchResult {
                Page = page,
                RowCount = rowCount,
                SearchValue = searchValue,
                PageSize = PAGE_SIZE,
                Data = data
            };
            return View("Index", supplierSearchResult);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo nhà cung cấp";
            Supplier supplier = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit", supplier);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa nhà cung cấp";
            Supplier? supplier = CommonDataService.GetSupplier(id);
            if (supplier == null)
            {
                return RedirectToAction("Index");
            }
            return View(supplier);
        }
        public IActionResult Delete(int id = 0)
        {
            Supplier? supplier = CommonDataService.GetSupplier(id);
            if (supplier == null)
                return RedirectToAction("Index");

            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            ViewBag.AllowDelete = !CommonDataService.IsUsedSupplier(id);
            return View(supplier);
        }

        [HttpPost]
        public IActionResult Save(Supplier supplier)
        {
            if (supplier.SupplierID == 0)
            {
                CommonDataService.AddSupplier(supplier);
            } else
            {
                CommonDataService.UpdateSupplier(supplier);
            }
            return RedirectToAction("Index");
        }
    }
}
