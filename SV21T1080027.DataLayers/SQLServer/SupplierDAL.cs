using Dapper;
using SV21T1080027.DomainModels;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Numerics;

namespace SV21T1080027.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString) { }

        public int Add(Supplier item)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    INSERT INTO Suppliers(SupplierName, ContactName, Province, Address, Phone, Email) 
                    VALUES (@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);

                    SELECT @@IDENTITY
                ";
                var param = new
                {
                    SupplierName = item.SupplierName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? ""
                };
                id = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text);
                connection.Close();
            };
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT count(*) 
                    FROM Suppliers
                    WHERE SupplierName LIKE @searchValue OR ContactName LIKE @searchValue
                ";
                var param = new
                {
                    searchValue = $"%{searchValue}%"
                };
                count = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    DELETE FROM Suppliers
                    WHERE SupplierID = @SupplierID
                ";
                var param = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? supplier = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT * 
                    FROM Suppliers
                    WHERE SupplierID = @SupplierID 
                ";
                var param = new
                {
                    SupplierID = id
                };
                supplier = connection.QueryFirstOrDefault<Supplier>(sql, param, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return supplier;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT COUNT(*) 
                    FROM Products 
                    WHERE SupplierID = @SupplierID
                ";
                var param = new
                {
                    SupplierID = id
                };
                result = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> result = new List<Supplier>();
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT * 
                            FROM (
		                            SELECT *, 
			                            ROW_NUMBER() OVER (ORDER BY SupplierName) AS RowNumber
		                            FROM Suppliers
		                            WHERE SupplierName LIKE @searchValue OR ContactName LIKE @searchValue 
	                            ) AS t
                            WHERE @pageSize = 0
	                            OR (RowNumber BETWEEN (@page - 1) * @pageSize + 1 AND @page * @pageSize)
                            ORDER BY RowNumber";
                var param = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = $"%{searchValue}%"
                };
                result = connection.Query<Supplier>(sql, param, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return result;
        }

        public bool Update(Supplier item)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                        UPDATE Suppliers
                        SET SupplierName = @SupplierName,
	                        ContactName = @ContactName,
	                        Province =  @Province,
	                        Address =  @Address,
	                        Phone = @Phone,
	                        Email = @Email
                        WHERE SupplierID = @SupplierID
                       ";
                var param = new
                {
                    SupplierName = item.SupplierName,
                    ContactName = item.ContactName,
                    Province = item.Province,
                    Address = item.Address,
                    Phone = item.Phone,
                    Email = item.Email,
                    SupplierID = item.SupplierID
                };
                result = connection.Execute(sql, param, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
