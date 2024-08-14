using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web.Models;
using System.Buffers;

namespace SV21T1080027.Web.Controllers
{
    public class CustomerController : Controller
    {
        const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "customer_search";
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = searchValue
                };
            }
            return View(input);

            
        }

        public IActionResult Search(PaginationSearchInput input)
        {

            Console.WriteLine("passing line 36 in search action: " + input.Page);
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, PAGE_SIZE, input.SearchValue);
            CustomerSearchResult customerSR = new CustomerSearchResult
            {
                Page = input.Page,
                RowCount = rowCount,
                SearchValue = input.SearchValue,
                PageSize = input.PageSize,
                Data = data
            };

            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(customerSR);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Tạo khách hàng";
            Customer customer = new Customer()
            {
                CustomerID = 0
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
            
            ViewBag.Title = (customer.CustomerID == 0) ? "Tạo khách hàng" : "Chỉnh sửa khách hàng";

            if (string.IsNullOrEmpty(customer.CustomerName))
                ModelState.AddModelError(nameof(customer.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrEmpty(customer.ContactName))
                ModelState.AddModelError(nameof(customer.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrEmpty(customer.Province))
                ModelState.AddModelError(nameof(customer.Province), "Vui lòng chọn tỉnh thành");
            
            customer.Phone = customer.Phone ?? "";
            customer.Email = customer.Email ?? "";
            customer.Address = customer.Address ?? "";

            // Nếu tồn tại lỗi thì trả dữ liệu về lại cho view để người sử dụng nhập lại cho đúng
            if (!ModelState.IsValid)
            {
                return View("Edit", customer);
            }

            if (customer.CustomerID == 0)
            {
                CommonDataService.AddCustomer(customer);
            } else
            {
                Console.WriteLine("cap nhat id = " + customer.CustomerID);
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
