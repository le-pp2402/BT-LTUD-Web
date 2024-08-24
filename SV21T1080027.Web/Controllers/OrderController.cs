using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web.Models;

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
                    DateRange = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", DateTime.Today.AddYears(-10), DateTime.Today)
                };
            }
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);
            return View(input);
        }

        public IActionResult Search(OrderSearchInput searchInp)
        { 
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

            Console.WriteLine(model.ToJson());

            return View(model);
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
            return RedirectToAction("Details", new { id = id });
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
            return RedirectToAction("Details", new { id = id });
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
            return RedirectToAction("Details", new { id = id });
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
            return RedirectToAction("Details", new { id = id });
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
                return RedirectToAction("Details", new { id = id });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderID = id;
            return View();
        }

        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
            {
                return Json("Vui lòng chọn người giao hàng");
            }

            bool result = OrderDataService.ShipOrder(id, shipperID);

            if (!result)
            {
                return Json("Đơn hàng này không cho phép đổi người giao hàng");
            }
            return Json("success");
        }

        /// <summary>
        /// Xoá mặt hàng ra khỏi đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public IActionResult DeleteDetail(int id = 0, int productID = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productID);

            if (!result)
            {
                TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";
            }

            return RedirectToAction("Details", new {id});
        }

        /// <summary>
        /// Giao diện để sửa đổi thông tin mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="id">Mã đơn hàng</param>
        /// <param name="productId">Mã mặt hàng</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);
            return View(model);
        }

        
        public IActionResult UpdateAddress(int OrderID, string DeliveryProvince, string DeliveryAddress)
        {
            Console.WriteLine(OrderID);
            Console.WriteLine(DeliveryAddress);
            Console.WriteLine(DeliveryProvince);

            if (DeliveryAddress.IsNullOrEmpty())
            {
                ModelState.AddModelError("DeliveryAddress", "Địa chỉ giao hàng không được để trống");
            }
            
            if (DeliveryProvince.IsNullOrEmpty() || DeliveryProvince.Equals("0"))
            {
                ModelState.AddModelError("DeliveryProvince", "Tỉnh thành không hợp lệ");
            }

            if (!ModelState.IsValid)
            {
                var model = OrderDataService.GetOrder(OrderID);

                if (model == null) return Redirect("Index");

                model.DeliveryAddress = DeliveryAddress;
                model.DeliveryProvince = DeliveryProvince;
                var details = OrderDataService.ListOrderDetails(OrderID);
                return View("Details", new OrderDetailModel() { Order = model, Details = details });
            }
            else
            {
                try
                {
                    OrderDataService.UpdateAddress(OrderID, DeliveryProvince, DeliveryAddress);
                } catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }
                return RedirectToAction("Details", new { id = OrderID });
            }
        }

        [HttpPost]
        public IActionResult UpdateDetail(int orderID, int productID, int quantity, string _salePrice)
        {
            if (quantity <= 0)
                return Json("Số lượng không hợp lệ");
            decimal salePrice = -1;
            try
            {
                salePrice = Convert.ToDecimal(_salePrice.Replace(",", ""));
                if (salePrice < 0) throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json("Giá bán không hợp lệ");
            }
            
            bool result = OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);    

            if (!result)
            {
                return Json("Không được phép thay đổi thông tin của đơn hàng này");
            }

            return Json("");
        }

        // Số dòng trên một trang hiện thị mặt hàng cần tìm khi lập đơn hàng
        private const int PRODUCT_PAGE_SIZE = 5;
        // Biến session lưu điều kiệm tìm kiếm mặt hàng khi lập đơn hàng
        private const string PRODUCT_SEARCH = "product_search_for_sale";
        // Biên session dùng để lưu giỏ hàng
        private const string SHOPPING_CART = "shopping_cart";


        /// <summary>
        /// Giao diện lập đơn hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput() { 
                    Page = 1, 
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(input);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm để thêm vào đơn hàng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IActionResult SearchProduct(ProductSearchInput input)
        {
            int rowCount = 0;
            var products = ProductDataService.List(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ProductSearchResult() { 
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = products
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }

        /// <summary>
        /// Lấy sản phẩn trong giỏ hàng từ session
        /// </summary>
        /// <returns></returns>
        public List<OrderDetail> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<OrderDetail>();
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return shoppingCart;
        }

        /// <summary>
        /// Hiển thị các mặt hàng đang có trong giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowShoppingCart()
        {
            var model = GetShoppingCart();
            return View(model);
        }

        /// <summary>
        /// Bổ sung thêm mặt hàng vào giỏ hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IActionResult AddToCart(OrderDetail data)
        {
            if (data.SalePrice < 0 || data.Quantity <= 0)
            {
                return Json("Giá bán hoặc số lượng không hợp lệ");
            }

            var shoppingCart = GetShoppingCart();
            
            bool isExisted = false;
            foreach(var elem in shoppingCart)
            {
                if (elem.ProductID == data.ProductID)
                {
                    isExisted = true;
                    elem.Quantity += data.Quantity;
                    elem.SalePrice = data.SalePrice;
                    break;
                }
            }

            if (!isExisted)
            {
                shoppingCart.Add(data);
            }

            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var shoppingCart = GetShoppingCart();

            foreach(var elem in shoppingCart)
            {
                if (elem.ProductID == id)
                {
                    shoppingCart.Remove(elem);
                    break;
                }
            }

            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống, không thể lập đơn hàng");
            }

            if (customerID <= 0 || string.IsNullOrEmpty(deliveryAddress) || string.IsNullOrEmpty(deliveryProvince))
            {
                return Json("Vui lòng nhập đầy đủ thông tin");
            }

            int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);

            try
            {
                int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, shoppingCart);
                ClearCart();
                return Json("");
            } catch (Exception e) 
            {
                return Json(e.Message);
            }
        }
    }
}
