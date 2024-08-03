using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
namespace SV21T1080027.Web.Controllers
{
    public class CustomerController : Controller
    {
        const int PAGE_SIZE = 20;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, page, PAGE_SIZE, searchValue);
            
            int pageCount = 1;
            
            pageCount = (rowCount + PAGE_SIZE - 1) / PAGE_SIZE;

            ViewBag.Page = page;
            ViewBag.PageCount = pageCount;
            ViewBag.RowCount = rowCount;
            ViewBag.SearchValue = searchValue;
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo khách hàng";
            Customer customer = new Customer()
            {
                CustomerId = 0
            };
            return View("Edit", customer);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa khách hàng";
            Customer? customer = CommonDataService.GetCustomer(id);
            if (customer == null)
            {
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult Save(Customer customer) { 
            // TODO: Validate information of customer
            if (customer.CustomerId == 0)
            {
                CommonDataService.AddCustomer(customer);
            } else
            {
                Console.WriteLine("cap nhat id = " + customer.CustomerId + " " + customer.Province);
                CommonDataService.UpdateCustomer(customer);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            Customer? customer = CommonDataService.GetCustomer(id);
            if (customer == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !CommonDataService.IsUsedCustomer(id);
            
            return View(customer);
        }


    }
}
