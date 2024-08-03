using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080027.DataLayers.SQLServer
{
    /// <summary>
    /// Lớp đóng vai trò "cha" cho các lớp cài đặt các phép xử lí dữ liệu
    /// trên cơ sở dữ liệu SQL Server
    /// </summary>
    public abstract class _BaseDAL
    {
        protected string _connectionString = "";
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionString"></param>
        public _BaseDAL(string connectionString) 
        { 
            _connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở kết nối tới CSDL
        /// </summary>
        /// <returns></returns>
        public SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();  
            return connection;
        }

    }
}
