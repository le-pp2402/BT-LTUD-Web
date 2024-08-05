using Dapper;
using SV21T1080027.DomainModels;

namespace SV21T1080027.DataLayers.SQLServer
{
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                            INSERT INTO Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked) 
                            VALUES(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                            
                            SELECT @@IDENTITY";
                var parameter = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address =  data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };

                id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                        SELECT COUNT(*)
                        FROM Customers 
                        WHERE (CustomerName LIKE @searchValue) OR (ContactName LIKE @searchValue)";
                var parameter = new {searchValue = $"%{searchValue}%"};
                count = connection.ExecuteScalar<int> (sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"DELETE FROM Customers WHERE CustomerID = @CustomerID";
                var param = new
                {
                    CustomerID = id
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();         
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * FROM Customers WHERE CustomerID = @CustomerID";
                var param = new
                {
                    CustomerID = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql, param, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS(SELECT * FROM Orders WHERE CustomerID = @CustomerID) 
                                SELECT 1
                            ELSE
                                SELECT 0";
                var param = new
                {
                    CustomerID = id
                };
                result = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public IList<Customer> List(int page, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT * 
                            FROM (
		                            SELECT *, 
			                            ROW_NUMBER() OVER (ORDER BY CustomerName) AS RowNumber
		                            FROM Customers
		                            WHERE CustomerName LIKE @searchValue OR ContactName LIKE @searchValue
	                            ) AS t
                            WHERE @pageSize = 0
	                            OR (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                            ORDER BY RowNumber;";
                var parameter = new
                {   
                    // têm tham số trong câu lệnh sql = giá trị của tham số sql
                    page = page,
                    pageSize = pageSize,
                    searchValue = $"%{searchValue}%"
                };
                data = connection.Query<Customer>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Customers
                            SET CustomerName = @CustomerName,
                                ContactName  = @ContactName, 
                                Province     = @Province, 
                                Address      = @Address, 
                                Phone        = @Phone, 
                                Email        = @Email, 
                               IsLocked      = @IsLocked
                            WHERE CustomerID = @CustomerID";

                var param = new
                {
                    CustomerID = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                result = connection.ExecuteScalar<int>(sql: sql, param: param, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }
    }
}
