using Dapper;
using SV21T1080027.DomainModels;
using System.Data;

namespace SV21T1080027.DataLayers.SQLServer
{
    public class EmployeeDAL : _BaseDAL, ICommonDAL<Employee>
    {
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Employee employee)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    INSERT INTO Employees(FullName, BirthDate, [Address], Phone, Email, Photo, IsWorking) 
                    VALUES (@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking);

                    SELECT @@IDENTITY
                ";
                var param = new
                {
                    FullName = employee.FullName ?? "",
                    BirthDate = employee.BirthDate,
                    Address = employee.Address ?? "",
                    Phone = employee.Phone ?? "",
                    Email = employee.Email ?? "",
                    Photo = employee.Photo ?? "",
                    IsWorking = employee.IsWorking
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
                    FROM Employees
                    WHERE FullName LIKE @searchValue
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
                    DELETE FROM Employees
                    WHERE EmployeeID = @EmployeeID
                ";
                var param = new
                {
                    EmployeeID = id
                };
                result = connection.Execute(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Employee? Get(int id)
        {
            Employee? employee = null;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT * 
                    FROM Employees
                    WHERE EmployeeId = @EmployeeId 
                ";
                var param = new
                {
                    EmployeeId = id
                };
                employee = connection.QueryFirstOrDefault<Employee>(sql, param, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return employee;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                    SELECT COUNT(*) 
                    FROM Orders 
                    WHERE EmployeeID = @EmployeeID
                ";
                var param = new
                {
                    EmployeeID = id
                };
                result = connection.ExecuteScalar<int>(sql, param, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public IList<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> result = new List<Employee>();
            using (var connection = OpenConnection()) 
            {
                var sql = @"
                            SELECT * 
                            FROM (
		                            SELECT *, 
			                            ROW_NUMBER() OVER (ORDER BY FullName) AS RowNumber
		                            FROM Employees
		                            WHERE FullName LIKE @searchValue 
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
                result = connection.Query<Employee>(sql, param, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return result;
        }

        public bool Update(Employee employee)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"
                        UPDATE Employees
                        SET FullName = @FullName,
	                        BirthDate = @BirthDate,
	                        Address =  @Address,
	                        Phone =  @Phone,
	                        Email = @Email,
	                        IsWorking = @IsWorking,
	                        Photo = @Photo
                        WHERE EmployeeID = @EmployeeID
                       ";
                var param = new
                {
                    FullName = employee.FullName ?? "",
                    BirthDate = employee.BirthDate,
                    Address = employee.Address ?? "",
                    Phone = employee.Phone ?? "",
                    Email = employee.Email ?? "",
                    IsWorking = employee.IsWorking,
                    Photo = employee.Photo ?? "",
                    EmployeeID = employee.EmployeeID
                };
                result = connection.Execute(sql, param, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
