using SV21T1080027.DataLayers.SQLServer;
using SV21T1080027.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080027.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly ProductDAL productDAL;
        
        static ProductDataService()
        {
            productDAL = new DataLayers.SQLServer.ProductDAL(Configuration.ConnectionString);
        }
        
        // PRODUCT
        /// <summary>
        /// Tìm kiếm là lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page"> trang cần hiển thị </param> 
        /// <param name="pageSize"> số sản phẩm hiện thị trong một trang (0 nếu không phân trang) </param>
        /// <param name="searchValue"> giá trị tìm kiếm (chuỗi rỗng nếu không tìm kiếm) </param>
        /// <param name="categoryID"> mã loại sản phẩm cần tìm (0 nếu không tìm theo loại sản phẩm) </param>
        /// <param name="supplierID"> mã nhà cũng cấp cần tìm (0 nếu không tìm thấy nhà cung cấp) </param>
        /// <param name="minPrice"> Mức giá nhỏ nhất cần tìm </param>
        /// <param name="maxPrice"> Mức giá lớn nhất cần tìm (0 nếu không hạn chế mức giá lớn nhất) </param>
        /// <returns></returns>
        public static List<Product> List(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDAL.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
            return productDAL.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice).ToList();
        }

        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maximumPrice"></param>
        /// <returns></returns>
        public static int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            return productDAL.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
        }

        /// <summary>
        /// Lấy thông tin mặt hàng theo mã mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Product? Get(int id)
        {
            return productDAL.Get(id);
        }

        /// <summary>
        /// Bổ sung mặt hàng mới (Trả về mã của mặt hàng mới được thêm)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int Add(Product data)
        {
            return productDAL.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool Update(Product data)
        {
            return productDAL.Update(data);
        }

        /// <summary>
        /// Xoá mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool Delete(int productID)
        {
            return productDAL.Delete(productID);
        }

        /// <summary>
        /// Kiểm tra xem mặt hàng có đơn hàng liên quan không
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public static bool IsUsed(int ProductID)
        {
            return productDAL.IsUsed(ProductID);
        }

        // PHOTO

        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (Sắp xếp theo thứ tự DisplayOrder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return productDAL.ListPhotos(productID).ToList();
        }

        /// <summary>
        /// Lấy thông tin ảnh dựa trên photoID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static ProductPhoto? GetPhoto(int photoID)
        {
            return productDAL.GetPhoto(photoID);
        }

        /// <summary>
        /// Bổ sung 1 ảnh cho một mặt hàng (hàm trả về mã ảnh vừa được bổ sung)
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto photo)
        {
            return productDAL.AddPhoto(photo);
        }

        /// <summary>
        /// Cập nhật ảnh của một mặt hàng
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static bool UpdatePhoto(ProductPhoto photo)
        {
            return productDAL.UpdatePhoto(photo);
        }

        /// <summary>
        /// Xoá ảnh của một mặt hàng
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long photoID)
        {
            return productDAL.DeletePhoto(photoID);
        }

        // ATTRIBUTE

        /// <summary>
        /// Lấy danh sách các thuộc tính của một mặt hàng, sắp xếp theo thứ tự DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return productDAL.ListAttributes(productID).ToList();
        }

        /// <summary>
        /// Thêm thuộc tính
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddAttribute(ProductAttribute data)
        {
            return productDAL.AddAttribute(data);
        }

        /// <summary>
        /// Lấy thông tin của một thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return productDAL.GetAttribute(attributeID);
        }

        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool UpdateAttribute(ProductAttribute attribute)
        {
            return productDAL.UpdateAttribute(attribute);
        }
        /// <summary>
        /// Xoá thuộc tính của mặt hàng dựa vào mã thuộc tín
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDAL.DeleteAttribute(attributeID);
        }
    }
}
