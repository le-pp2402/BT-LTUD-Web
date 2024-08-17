using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1080027.BusinessLayers;
using SV21T1080027.DomainModels;
using SV21T1080027.Web.Models;
using System.Buffers;
using SV21T1080027.Web;

namespace SV21T1080027.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "product_search";

        public IActionResult Index(int page = 1, int categoryID = 0, int supplierID = 0, int minPrice = 0, int maxPrice = 0, string searchValue = "")
        {
            ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(SEARCH_CONDITION);

            if (input == null)
            {
                input = new ProductSearchInput
                {
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    SearchValue = searchValue,
                    Page = 1,
                    PageSize = PAGE_SIZE
                };
            }


            return View(input);
        }

        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = ProductDataService.List(out rowCount, input.Page, input.PageSize, input.SearchValue, input.CategoryID, input.SupplierID, input.MinPrice, input.MaxPrice).ToList();

            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data,
                MaxPrice = input.MaxPrice,
                MinPrice = input.MinPrice,
                SupplierID = input.SupplierID,
                CategoryID = input.CategoryID
            };

            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Thêm sản phẩm";

            var productDetail = new ProductDetail()
            {
                Product = new Product()
            };
            return View("Edit", productDetail);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa sản phẩm";
            
            var product = ProductDataService.Get(id);
            if (product == null)
            {
                return View("Index");
            }

            var lstPhoto = ProductDataService.ListPhotos(id);
            var lstAttribute = ProductDataService.ListAttributes(id);

            var productDetail = new ProductDetail()
            {
                Photos = lstPhoto ?? new List<ProductPhoto>(),
                Attributes = lstAttribute ?? new List<ProductAttribute>(),
                Product = product    
            };

            return View(productDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Save(Product product, IFormFile uploadPhoto, String _Price)
        {

            ViewBag.Title = (product.ProductID == 0) ? "Thêm sản phẩm" : "Chỉnh sửa sản phẩm";

            if (string.IsNullOrEmpty(product.ProductName))
                ModelState.AddModelError(nameof(product.ProductName), "Tên sản phẩm không được để trống");
            if (product.CategoryID == 0)
                ModelState.AddModelError(nameof(product.CategoryID), "Vui lòng chọn loại sản phẩm");
            if (product.SupplierID == 0)
                ModelState.AddModelError(nameof(product.SupplierID), "Vui lòng chọn nhà cung cấp");
            if (string.IsNullOrEmpty(_Price))
            {
                ModelState.AddModelError(nameof(_Price), "Giá trị không hợp lệ");
            }
            product.Photo = product.Photo ?? "";
            product.Unit = product.Unit ?? "";

            try
            {
                product.Price = Decimal.Parse(_Price.Replace(",", ""));
            } catch (Exception e)
            {
                ModelState.AddModelError(nameof(_Price), "Giá trị không hợp lệ");
                Console.WriteLine(e.Message);
            }
            
            if (!ModelState.IsValid)
            {
                var lstPhoto = ProductDataService.ListPhotos(product.ProductID);
                var lstAttribute = ProductDataService.ListAttributes(product.ProductID);
                var productDetail = new ProductDetail()
                {
                    Photos = lstPhoto ?? new List<ProductPhoto>(),
                    Attributes = lstAttribute ?? new List<ProductAttribute>(),
                    Product = product
                };
                return View("Edit", productDetail);
            }

            if (uploadPhoto != null && uploadPhoto.ContentType.Contains("image"))
            {
                var fileName = Path.GetFileName(product.ProductID.ToString() + Path.GetExtension(uploadPhoto.FileName));
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                Console.WriteLine(fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }

                product.Photo = $"/images/products/{fileName}";
            }

            if (product.ProductID == 0)
            {
                product.ProductID = ProductDataService.Add(product);
            } else
            {
                ProductDataService.Update(product);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.Delete(id);
                return RedirectToAction("Index");
            } 

            Product? product = ProductDataService.Get(id);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.AllowDelete = true;
            if (ProductDataService.IsUsed(id))
            {
                ViewBag.AllowDelete = false;
            }
            return View(product);
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    ProductPhoto photo = new ProductPhoto
                    {
                        ProductID = id,
                        PhotoID = 0,
                        Description = "",
                        DisplayOrder = 100,
                        IsHidden = false,
                        Photo = ""
                    };
                    return View(photo);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    ProductPhoto? img = ProductDataService.GetPhoto(photoId);
                    if (img == null) return RedirectToAction("Edit", new { id = id });
                    return View(img);
                case "delete":
                    ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new {id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var modelAdd = new ProductAttribute
                    {
                        AttributeID = 0,
                        AttributeValue = "",
                        DisplayOrder = 0,
                        AttributeName = "",
                        ProductID = id
                    };
                    return View(modelAdd);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    var modelEdit = ProductDataService.GetAttribute(attributeId);
                    if (modelEdit == null) return RedirectToAction("Edit", new { id = id });
                    return View(modelEdit);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePhoto(ProductPhoto productPhoto, IFormFile uploadPhoto)
        {
            ViewBag.Title = (productPhoto.PhotoID == 0) ? "Thêm ảnh" : "Chỉnh sửa ảnh";

            if (productPhoto.Equals("") && (uploadPhoto == null || !uploadPhoto.ContentType.Contains("image")))
                ModelState.AddModelError(nameof(productPhoto.Photo), "Chưa có ảnh hoặc ảnh không hợp lệ");
            
            if (!ModelState.IsValid)
            {
                if (productPhoto.PhotoID == 0) {
                    return RedirectToAction("Photo", new { id = productPhoto.ProductID, method = "add", photoId = 0 });
                } 
                else
                {
                    return RedirectToAction("Photo", new { id = productPhoto.ProductID, method = "edit", photoId = productPhoto.PhotoID });
                }
            }

            productPhoto.Photo = productPhoto.Photo ?? "";
            productPhoto.Description = productPhoto.Description ?? "";

            if (productPhoto.PhotoID == 0)
            {
                productPhoto.PhotoID = ProductDataService.AddPhoto(productPhoto);
            }

            if (uploadPhoto != null && uploadPhoto.ContentType.Contains("image"))
            {
                var fileName = Path.GetFileName(productPhoto.PhotoID.ToString() + Path.GetExtension(uploadPhoto.FileName));
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products/img", fileName);

                Console.WriteLine(fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadPhoto.CopyToAsync(stream);
                }

                productPhoto.Photo = $"/images/products/img/{fileName}";
            }

            ProductDataService.UpdatePhoto(productPhoto);

            return RedirectToAction("Edit", new { id = productPhoto.ProductID });
        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute productAttribute)
        {
            ViewBag.Title = (productAttribute.AttributeID == 0) 
                ? "Bổ sung thuộc tính cho mặt hàng " : "Chỉnh sửa thuộc tính cho mặt hàng";

            productAttribute.AttributeValue = productAttribute.AttributeValue ?? "";
            productAttribute.AttributeName = productAttribute.AttributeName ?? "";

            if (productAttribute.AttributeID == 0)
                ProductDataService.AddAttribute(productAttribute);
            else
                ProductDataService.UpdateAttribute(productAttribute);

            return RedirectToAction("Edit", new { id = productAttribute.ProductID });
        }
    }
}
