using SV21T1080027.DataLayers;
using SV21T1080027.DomainModels;
namespace SV21T1080027.BusinessLayers
{
    public static class CommonDataService
    {
        static readonly ICommonDAL<Province> provinceDB; 
        static readonly ICommonDAL<Customer> customerDB;    
        static CommonDataService() {
            provinceDB = new DataLayers.SQLServer.ProvinceDAL(Configuration.ConnectionString);
            customerDB = new DataLayers.SQLServer.CustomerDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// Trả về danh sách của tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }

        /// <summary>
        /// Danh sách khách hàng (tìm kiếm, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page = 1, int pageSize = 1, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Danh sách khách hàng (tìm kiếm, không phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(string searchValue = "")
        {
            return customerDB.List(1, 0, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của một khách hàng dựa vào mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Thông tin khách hàng nếu không tồn tại trả về null</returns>
        public static Customer? GetCustomer(int id)
        {
            if (id < 0) 
                return null;
            return customerDB.Get(id);
        }

        /// <summary>
        /// Bổ sung 1 khách hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Trả về id của khách hàng vừa được bổ sung</returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của 1 khách hàng 
        /// </summary>
        /// <param name="data"></param>
        /// <returns> true nếu cập nhật thành công, false nếu thất bại </returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }

        /// <summary>
        /// Xoá 1 khách hàng dựa vào mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true nếu xoá thành công, false nếu thất bại </returns>
        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }

        /// <summary>
        /// Kiếm tra khách có các thông tin liên quan ở các bảng khác không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
    }
}

// ?? lớp static là gì 
// Constructor trong lớp static có đặc điểm gì, Được gọi khi nào 
