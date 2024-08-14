using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
        public IActionResult Save(Employee employee, string birthDateInput, IFormFile uploadPhoto)
        {

            ViewBag.Title = employee.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

            //Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(employee.FullName))
                ModelState.AddModelError(nameof(employee.FullName), "Họ tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(employee.Email))
                ModelState.AddModelError(nameof(employee.Email), "Vui lòng nhập email");
            if (string.IsNullOrWhiteSpace(employee.Address))
                employee.Address = "";
            if (string.IsNullOrWhiteSpace(employee.Phone))
                employee.Phone = "";

            //Xử lý ngày sinh
            DateTime? birthDate = birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                employee.BirthDate = birthDate.Value;

            //Xử lý với ảnh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee)
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\employees"); //đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); //Đường dẫn đến file cần lưu D:\images\employees\photo.png

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                employee.Photo = fileName;
            }

            if (!ModelState.IsValid)
                return View("Edit", employee);

            if (employee.EmployeeID == 0)
            {
                CommonDataService.AddEmployee(employee);
            }
            else
            {
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
