using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
namespace SV21T1080027.Web.Controllers
{
    public class EmployeeController : Controller
    {
        const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "employee_search"; //Tên biến dùng để lưu trong session

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput> (SEARCH_CONDITION);
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
            //int rowCount = 0;
            //var data = CommonDataService.ListOfEmployees(out rowCount, page, PAGE_SIZE, searchValue);

            //EmployeeSearchResult employeeSearchResult = new EmployeeSearchResult
            //{
            //    Page = page,
            //    RowCount = rowCount,
            //    SearchValue = searchValue,
            //    PageSize = PAGE_SIZE,
            //    Data = data
            //};
            //return View(employeeSearchResult);
        }

        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
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
        public async Task<IActionResult> Save(Employee employee, IFormFile uploadPhoto)
        {
            if (employee.EmployeeID == 0)
            {
                employee.EmployeeID = CommonDataService.AddEmployee(employee);
            }
            
            Console.WriteLine("cap nhat id = " + employee.EmployeeID);
            if (uploadPhoto != null && uploadPhoto.ContentType.Contains("image"))
            {
                var fileName = Path.GetFileName(employee.EmployeeID.ToString() + Path.GetExtension(uploadPhoto.FileName));
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/employees", fileName);
                    
                Console.WriteLine(fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }

                employee.Photo = $"/images/employees/{fileName}";
            }
            CommonDataService.UpdateEmployee(employee);
            
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
