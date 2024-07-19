using System;
using Dapper;
using SV21T1080027.DomainModels;
using Microsoft.Data.SqlClient;

namespace SV21T1080027.DataLayers
{
    public class CustomerDAL
    {
        private string connectionString = "";
        public CustomerDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }
        /// <summary>
        ///  Lấy danh sách toàn bộ các khách hàng
        /// </summary>
        /// <returns></returns>
        public List<Customer> List()
        {
            List<Customer> list = new List<Customer>();
            try
            {
                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    var sqlStm = @"SELECT * FROM Customers";
                    list = connection.Query<Customer>(sql: sqlStm, commandType: System.Data.CommandType.Text).ToList();

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return list;
            
        }
    }
}
