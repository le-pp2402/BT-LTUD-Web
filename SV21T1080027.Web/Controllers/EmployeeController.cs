using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;

namespace SV21T1080027.Web.Controllers
{
    public class EmployeeController : Controller
    {
        const int PAGE_SIZE = 9;
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue);
            int pageCount = (rowCount + PAGE_SIZE - 1) / PAGE_SIZE;

            ViewBag.Page = page;
            ViewBag.PageCount = pageCount;
            ViewBag.RowCount = rowCount;
            ViewBag.SearchValue = searchValue;

            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Thêm nhân viên";
            Employee employee = new Employee()
            {
                EmployeeID = 0,
                IsWorking = false
            };
            return View("Edit", employee);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa nhân viên";
            Employee? employee = CommonDataService.GetEmployee(id);
            if (employee == null)
            {
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        [HttpPost]
        public IActionResult Save(Employee employee)
        {
            // TODO: Validate information of employee
            if (employee.EmployeeID == 0)
            {
                CommonDataService.AddEmployee(employee);
            }
            else
            {
                Console.WriteLine("cap nhat id = " + employee.EmployeeID);
                CommonDataService.UpdateEmployee(employee);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            Employee? employee = CommonDataService.GetEmployee(id);
            if (employee == null)
                return RedirectToAction("Index");

            if (Request.Method == "POST")
            {
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);

            return View(employee);
        }
    }
}
