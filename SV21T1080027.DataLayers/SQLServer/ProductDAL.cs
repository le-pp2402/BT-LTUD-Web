using Dapper;
using SV21T1080027.DomainModels;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1080027.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString) { }
        /// <summary>
        /// Thêm sản phẩm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                INSERT INTO Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                                VALUES(@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                                SELECT @@IDENTITY;
                              ";
                var param = new 
                {
                    ProductName = data.ProductName,
                    ProductDescription = data.ProductDescription,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling,
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID
                };  
                id = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        /// <summary>
        /// Thêm thuộc tính cho sản phẩm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                                VALUES(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                                SELECT @@IDENTITY;
                              ";
                var param = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName,
                    AttributeValue = data.AttributeValue,
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        /// <summary>
        /// Thêm ảnh
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public long AddPhoto(ProductPhoto photo)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                INSERT INTO ProductPhotos(Photo, Description, DisplayOrder, ProductID)
                                VALUES (@Photo, @Description, @DisplayOrder, @ProductID);
                                SELECT @@IDENTITY;
                              ";
                var param = new
                {
                    Photo = photo.Photo,
                    Description = photo.Description,
                    DisplayOrder = photo.DisplayOrder,
                    ProductID = photo.ProductID
                };
                id = connection.ExecuteScalar<long>(sql, param, commandType: System.Data.CommandType.Text);
            }
            return id;
        }

        /// <summary>
        /// Đếm số sản phẩm
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            using (var connection = OpenConnection())
            {
                string sql = @" SELECT COUNT(*)
                                FROM Products
                                WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                AND (Price >= @MinPrice)
                                AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                              ";
                var param = new
                {
                    SearchValue = $"%{searchValue}%",
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                count = connection.QueryFirstOrDefault<int>(sql, param, commandType: System.Data.CommandType.Text);
            }
            return count;
        }

        /// <summary>
        /// Xoá sản phẩm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                DELETE FROM Products WHERE ProductID = @ProductID
                              ";
                var param = new
                {
                    ProductID = productID
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        /// <summary>
        /// Xoá thuộc tính sản phẩm bằng mã sản phẩm
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                DELETE FROM ProductAttributes WHERE AttributeID = @AttributeID
                              ";
                var param = new
                {
                    AttributeID = attributeID
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        /// <summary>
        /// Xoá ảnh của sản phẩm
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                DELETE FROM ProductPhotos WHERE PhotoID = @PhotoID
                              ";
                var param = new
                {
                    PhotoID = photoID
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        /// <summary>
        /// Lấy thông tin sản phẩm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product? Get(int id)
        {
            Product? result = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT * 
                            FROM Products 
                            WHERE ProductID = @ProductID
                           ";
                var param = new
                {
                    ProductId = id
                };
                result = connection.QueryFirstOrDefault<Product>(sql, param, commandType: System.Data.CommandType.Text);
            }
            return result;
        }

        /// <summary>
        /// Lấy thông tin thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? result = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT * 
                            FROM ProductAttributes 
                            WHERE AttributeID = @AttributeID
                           ";
                var param = new
                {
                    AttributeID = attributeID
                };
                result = connection.QueryFirstOrDefault<ProductAttribute>(sql, param, commandType: System.Data.CommandType.Text);
            }
            return result;
        }

        /// <summary>
        /// Lấy hình ảnh dựa vào ảnh ID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public ProductPhoto? GetPhoto(int photoID)
        {
            ProductPhoto? result = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT * 
                            FROM ProductPhotos 
                            WHERE PhotoID = @PhotoID
                           ";
                var param = new
                {
                    PhotoID = photoID
                };
                result = connection.QueryFirstOrDefault<ProductPhoto>(sql, param, commandType: System.Data.CommandType.Text);
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra xem sản phẩm có bị sử dụng ở bảng khác không
        /// </summary>
        /// <param name="ProductID"></param>
        /// <returns></returns>
        public bool IsUsed(int ProductID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                DECLARE @PP INT = 0;
                                DECLARE @OD INT = 0;
                                DECLARE @PA INT = 0;

                                SELECT @PP = COUNT(*) FROM ProductPhotos WHERE ProductID = @ID;
                                SELECT @OD = COUNT(*) FROM OrderDetails WHERE ProductID = @ID;
                                SELECT @PA = COUNT(*) FROM ProductAttributes WHERE ProductID = @ID;

                                IF (@PP + @OD + @PA) > 1 
	                                SELECT 1
                                ELSE 
	                                SELECT 0
                             ";
                var param = new
                {
                    ID = ProductID
                };
                result = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text) == 1;
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo các thông số tìm kiếm
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> result = new List<Product>();
            using (var connection = OpenConnection()) {
                string sql = @"
                                SELECT *
                                FROM (
                                    SELECT *,
                                            ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
                                    FROM Products
                                    WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND (Price >= @MinPrice)
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                                ) AS t
                                WHERE (@PageSize = 0)
                                        OR (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)";
                var param = new
                {
                    SearchValue = $"%{searchValue}%",
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    Page = page,
                    PageSize = pageSize,
                };
                result = connection.Query<Product> (sql, param, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách thuộc tính, đã được sắp xếp theo thứ tự xuất hiện, của một sản phẩm
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> result = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                string sql = @"
                            SELECT * 
                            FROM ProductAttributes 
                            WHERE ProductID = @id
                            ORDER BY DisplayOrder ASC
                        ";
                var param = new 
                {
                    id = productID
                };
                result = connection.Query<ProductAttribute>(sql, param, commandType: System.Data.CommandType.Text).ToList();
            }   
            return result;
        }

        /// <summary>
        /// Lấy danh sách của một sản phẩn dựa vào id sản phẩm
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> result = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                string sql = @"
                                SELECT * 
                                FROM ProductPhotos 
                                WHERE ProductID = @id
                                ORDER BY DisplayOrder ASC
                            ";
                var param = new
                {
                    id = productID
                };
                result = connection.Query<ProductPhoto>(sql, param, commandType: System.Data.CommandType.Text).ToList();
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                UPDATE Products
                                SET ProductName = @ProductName, 
                                    ProductDescription = @ProductDescription, 
                                    SupplierID = @SupplierID, 
                                    CategoryID = @CategoryID, 
                                    Unit = @Unit, 
                                    Price = @Price, 
                                    Photo = @Photo, 
                                    IsSelling = @IsSelling
                                WHERE ProductID = @ProductID
                              ";
                var param = new
                {
                    ProductName = data.ProductName,
                    ProductDescription = data.ProductDescription,
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit,
                    Price = data.Price,
                    Photo = data.Photo,
                    IsSelling = data.IsSelling,
                    ProductID = data.ProductID
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin thuộc tính
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public bool UpdateAttribute(ProductAttribute attribute)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                UPDATE ProductAttributes
                                SET AttributeName = @AttributeName, 
                                    AttributeValue = @AttributeValue, 
                                    DisplayOrder = @DisplayOrder
                                WHERE AttributeID = @AttributeID
                              ";
                var param = new
                {
                    AttributeName = attribute.AttributeName,
                    AttributeValue = attribute.AttributeValue,
                    DisplayOrder = attribute.DisplayOrder,
                    AttributeID = attribute.AttributeID
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        /// <summary>
        /// Cập nhật thông tin ảnh
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public bool UpdatePhoto(ProductPhoto photo)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                string sql = @"
                                UPDATE ProductPhotos
                                SET Photo =@Photo, 
                                    Description = @Description, 
                                    DisplayOrder = @DisplayOrder
                                WHERE PhotoID = @PhotoID
                              ";
                var param = new
                {
                    Photo = photo.Photo,
                    Description = photo.Description,
                    DisplayOrder = photo.DisplayOrder,
                    PhotoID = photo.PhotoID
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }
    }
}
