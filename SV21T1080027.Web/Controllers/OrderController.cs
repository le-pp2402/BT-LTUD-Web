using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using NuGet.Protocol;
using SV21T1080027.BusinessLayers;
using SV21T1080027.Web.Models;
using System.CodeDom;

namespace SV21T1080027.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string ORDER_SEARCH = "order_search";

        public IActionResult Index()
        {
            OrderSearchInput? input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH);

            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    DateRange = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", DateTime.Today.AddMonths(-1), DateTime.Today)
                };
            }

            ApplicationContext.SetSessionData(ORDER_SEARCH, input);
            return View(input);
        }

        public IActionResult Search(OrderSearchInput searchInp)
        {
            Console.WriteLine(searchInp.ToJson());
            int rowCount = 0;

            searchInp.SearchValue = searchInp.SearchValue ?? "";

            var data = OrderDataService.ListOrders(out rowCount, searchInp.Page, searchInp.PageSize, searchInp.Status,
                    searchInp.FromTime, searchInp.ToTime, searchInp.SearchValue
                );

            OrderSearchResult model = new OrderSearchResult()
            {
                Page = searchInp.Page,
                PageSize = searchInp.PageSize,
                SearchValue = searchInp.SearchValue,
                Data = data,
                RowCount = rowCount,
                Status = searchInp.Status,
                TimeRange = searchInp.DateRange ?? ""
            };

            Console.WriteLine(model.ToJson());

            ApplicationContext.SetSessionData(ORDER_SEARCH, searchInp);
            return View(model);
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null) return RedirectToAction("Index");

            var details = OrderDataService.ListOrderDetails(id);

            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            }; 

            return View(details);
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái đã được duyệt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể duyệt đơn hàng này";
            }
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái kết thúc
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể ghi nhận trạng thái kế thúc cho đơn hàng này";
            }
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái huỷ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể thực hiện thao tác huỷ đối với đơn hàng này";
            }
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// Chuyển đơn hàng sang trạng thái từ chối
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id); 
            if (!result)
            {
                TempData["Message"] = "Không thể thực hiện thao tác từ chối đối với đơn hàng này";
            }
            return RedirectToAction("Detail", new { id = id });
        }

        /// <summary>
        /// Xoá đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể xoá đơn hàng này";
            }
            return RedirectToAction("Detail", new { id = id });
        }


        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            return View();
        }

        public IActionResult Shipping(int id = 0)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
