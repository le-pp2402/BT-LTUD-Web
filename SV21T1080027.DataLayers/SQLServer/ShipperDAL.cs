using Dapper;
using SV21T1080027.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080027.DataLayers.SQLServer
{
    public class ShipperDAL : _BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(String connectionString) : base(connectionString) { }

        public int Add(Shipper item)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    INSERT INTO Shippers(ShipperName, Phone) 
                    VALUES (@ShipperName, @Phone);

                    SELECT @@IDENTITY
                ";
                var param = new
                {
                    ShipperName = item.ShipperName,
                    Phone = item.Phone
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
                    FROM Shippers
                    WHERE ShipperName LIKE @searchValue OR Phone LIKE @searchValue
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
                    DELETE FROM Shippers
                    WHERE ShipperID = @ShipperID
                ";
                var param = new
                {
                    ShipperID = id
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? shipper = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT * 
                    FROM Shippers
                    WHERE ShipperID = @ShipperID 
                ";
                var param = new
                {
                    ShipperID = id
                };
                shipper = connection.QueryFirstOrDefault<Shipper>(sql, param, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return shipper;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT COUNT(*) 
                    FROM Orders 
                    WHERE ShipperID = @ShipperID
                ";
                var param = new
                {
                    ShipperID = id
                };
                result = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> result = new List<Shipper>();
            using (var connection = OpenConnection())
            {
                var sql = @"
                            SELECT * 
                            FROM (
		                            SELECT *, 
			                            ROW_NUMBER() OVER (ORDER BY ShipperName) AS RowNumber
		                            FROM Shippers
		                            WHERE ShipperName LIKE @searchValue OR Phone LIKE @searchValue
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
                result = connection.Query<Shipper>(sql, param, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return result;
        }

        public bool Update(Shipper item)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                        UPDATE Shippers
                        SET ShipperName = @ShipperName,
	                        Phone = @Phone
                        WHERE ShipperID = @ShipperID
                       ";
                var param = new
                {
                    ShipperName = item.ShipperName ?? "",
                    Phone = item.Phone ?? "",
                    ShipperID = item.ShipperID
                };
                result = connection.Execute(sql, param, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
