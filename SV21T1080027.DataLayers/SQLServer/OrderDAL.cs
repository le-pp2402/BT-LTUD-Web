using Azure;
using Dapper;
using SV21T1080027.DomainModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080027.DataLayers.SQLServer
{
    public class OrderDAL : _BaseDAL, IOrderDAL
    {
        public OrderDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Order data)
        {
            int id = 0;
            using (var cn = OpenConnection())
            {
                var sql = @"INSERT INTO Orders(
                                    CustomerId, 
                                    OrderTime, 
                                    DeliveryProvince, 
                                    DeliveryAddress,
                                    EmployeeID, 
                                    Status
                                    )
                            VALUES (
                                    @CustomerID, 
                                    GETDATE(),
                                    @DeliveryProvince, 
                                    @DeliveryAddress,
                                    @EmployeeID, 
                                    @Status
                            );
                            SELECT @@identity";
                var param = new
                {
                    CUstomerID = data.CustomerID,
                    DeliveryProvince = data.DeliveryProvince,
                    DeliveryAddress = data.DeliveryAddress,
                    EmployeeID = data.EmployeeID,
                    Status = Constants.ORDER_INIT
                };

                id = cn.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text);
            }
            return id;
        }

        public int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            {
                int count = 0;
                if (!string.IsNullOrEmpty(searchValue))
                    searchValue = $"%{searchValue}%";
                
                using (var cn = OpenConnection())
                {
                    var sql = @"SELECT COUNT(*)
                                FROM Orders AS o
                                LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                WHERE (@Status = 0 OR o.Status = @Status)
                                AND (@FromTime is null OR o.OrderTime >= @FromTime)
                                AND (@ToTime is null OR o.OrderTime <= @ToTime)
                                AND (@SearchValue = N''
                                OR c.CustomerName LIKE @SearchValue
                                OR e.FullName LIKE @SearchValue
                                OR s.ShipperName LIKE @SearchValue)";

                    var param = new {
                        Status = status,
                        FromTime = fromTime,
                        ToTime = toTime,
                        SearchValue = searchValue
                    };

                    count = cn.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text);
                }
                
                return count;
            }
        }

        public bool UpdateAddress(int OrderID, String DeliveryProvince, String DeliveryAddress)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = @"UPDATE Orders 
                            SET DeliveryProvince = @DeliveryProvince, DeliveryAddress = @DeliveryAddress
                            WHERE OrderID = @OrderID    
                            ";
                var param = new
                {
                    DeliveryProvince = DeliveryProvince,
                    DeliveryAddress = DeliveryAddress,
                    OrderID = OrderID
                };
                result = cn.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        public bool Delete(int orderID)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID;
                            DELETE FROM Orders WHERE OrderID = @OrderID";

                var param = new
                {
                    OrderID = orderID
                };
                result = cn.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        public bool DeleteDetail(int orderID, int productID)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = @"DELETE FROM OrderDetails
                            WHERE OrderID = @OrderID AND ProductID = @ProductID";
                var param = new
                {
                    OrderID = orderID,
                    ProductID = productID
                };
                result = cn.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        public Order? Get(int orderID)
        {
            Order? result = null;
            using (var cn = OpenConnection())
            {
                var sql = @"SELECT o.*, 
                            c.CustomerName, 
                            c.ContactName AS CustomerContactName, 
                            c.Address AS CustomerAddress, 
                            c.Phone AS CustomerPhone, 
                            c.Email AS CustomerEmail, 
                            e.FullName AS EmployeeName, 
                            s.ShipperName, 
                            s.Phone AS ShipperPhone 
                            FROM Orders AS o 
                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID 
                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID 
                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID 
                            WHERE o.OrderID = @OrderID";
                var param = new
                {
                    OrderID = orderID,
                };

                result = cn.QueryFirstOrDefault<Order?>(sql, param, commandType: System.Data.CommandType.Text);
            }

            return result;
        }

        public OrderDetail? GetDetail(int orderID, int productID)
        {
            OrderDetail? result = null;
            using (var cn = OpenConnection())
            {
                var sql = @"SELECT od.*, p.ProductName, p.Photo, p.Unit 
                            FROM OrderDetails AS od 
                            JOIN Products AS p ON od.ProductID = p.ProductID 
                            WHERE od.OrderID = @OrderID and od.ProductID = @ProductID";
                var param = new
                {
                    OrderID = orderID,
                    ProductID = productID
                };

                result = cn.QueryFirstOrDefault<OrderDetail?>(sql, param, commandType: System.Data.CommandType.Text);
            }

            return result;
        }

        public IList<Order> List(int page = 1, int pageSize = 0, int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            List<Order> list = new List<Order>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                var sql = @"WITH CTE AS 
                            ( 
                                SELECT row_number() over(order BY o.OrderTime desc) AS RowNumber, 
                                o.*, 
                                c.CustomerName, 
                                c.ContactName AS CustomerContactName, 
                                c.Address AS CustomerAddress, 
                                c.Phone AS CustomerPhone, 
                                c.Email AS CustomerEmail, 
                                e.FullName AS EmployeeName, 
                                s.ShipperName, 
                                s.Phone AS ShipperPhone 
                                FROM Orders AS o 
                                LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID 
                                LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID 
                                LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID 
                                WHERE (@Status = 0 OR o.Status = @Status) 
                                AND (@FromTime is null OR o.OrderTime >= @FromTime) 
                                AND (@ToTime is null OR o.OrderTime <= @ToTime) 
                                AND (@SearchValue = N'' 
                                OR c.CustomerName LIKE @SearchValue 
                                OR e.FullName LIKE @SearchValue 
                                OR s.ShipperName LIKE @SearchValue) 
                            ) 
                            SELECT * FROM CTE 
                            WHERE (@PageSize = 0) 
                            OR (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize) 
                            ORDER BY RowNumber 
                            ";
                var param = new
                {
                    Status = status,
                    FromTime = fromTime,
                    ToTime = toTime,
                    SearchValue = searchValue,
                    PageSize = pageSize,
                    Page = page
                };
                list = connection.Query<Order>(sql, param, commandType: System.Data.CommandType.Text).ToList();
            }
            return list;
        }

        public IList<OrderDetail> ListDetails(int orderID)
        {
            List<OrderDetail> list = new List<OrderDetail>();

            using (var connection = OpenConnection())
            {
                var sql = @"
                            select od.*, p.ProductName, p.Photo, p.Unit
                            from OrderDetails as od
                            join Products as p on od.ProductID = p.ProductID
                            where od.OrderID = @OrderID";

                var param = new
                {
                    OrderID = orderID
                };

                list = connection.Query<OrderDetail>(sql, param, commandType: System.Data.CommandType.Text).ToList();
            }
            return list;
        }

        public bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS ( SELECT * FROM OrderDetails 
                            WHERE OrderID = @OrderID AND ProductID = @ProductID) 
                            UPDATE OrderDetails 
                            SET Quantity = @Quantity, 
                            SalePrice = @SalePrice 
                            WHERE OrderID = @OrderID AND ProductID = @ProductID 
                            ELSE 
                            INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice) 
                            VALUES (@OrderID, @ProductID, @Quantity, @SalePrice) ";

                var param = new
                {
                    OrderID = orderID,
                    ProductID = productID,
                    Quantity = quantity,
                    SalePrice = salePrice
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        public bool Update(Order data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            UPDATE Orders 
                            SET CustomerID = @CustomerID, 
                            OrderTime = @OrderTime, 
                            DeliveryProvince = @DeliveryProvince, 
                            DeliveryAddress = @DeliveryAddress, 
                            EmployeeID = @EmployeeID, 
                            AcceptTime = @AcceptTime, 
                            ShipperID = @ShipperID, 
                            ShippedTime = @ShippedTime, 
                            FinishedTime = @FinishedTime, 
                            Status = @Status 
                            WHERE OrderID = @OrderID";
                var param = new
                {
                    CustomerID = data.CustomerID,
                    OrderTime = data.OrderTime,
                    DeliveryProvince = data.DeliveryProvince,
                    DeliveryAddress = data.DeliveryAddress,
                    EmployeeID = data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status,
                    OrderID = data.OrderID
                };

                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }
    }
}
