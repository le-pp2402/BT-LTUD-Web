namespace SV21T1080027.DataLayers
{
    /// <summary>
    ///  định nghĩa các phép xử lí dữ liệu chung
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommonDAL<T> where T : class
    {   
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu dưới dạng phân trang.
        /// </summary>
        /// <param name="page"> Trang cần hiển thị </param>
        /// <param name="pageSize"> Số dòng cần hiển thị (bằng 0 nếu không phân trang) </param>
        /// <param name="searchValue">Giá trị tìm kiếm (chuỗi rỗng nếu không tìm kiếm) </param>
        /// <returns></returns>
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        /// <summary>
        /// Đếm số dòng dữ liệu tìm được (chuỗi rỗng nếu không tìm kiếm).
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        int Count(String searchValue = "");
        /// <summary>
        /// Lấy một dòng dữ liệu dựa trên id. 
        /// </summary>
        /// <param name="id"> Mã của dữ liệu </param>
        /// <returns></returns>
        T? Get(int id);
        /// <summary>
        /// Bổ sung dữ liệu vào bảng hàm trả về id của dữ liệu vừa bổ sung.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int Add(T item);
        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Update(T item);
        /// <summary>
        /// Xoá một dòng dữ liệu dựa vào id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);
        /// <summary>
        /// Kiếm tra xem một dòng dữ liệu có mã id liệu có dữ liệu liên quan 
        /// ở các bảng khác hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
    }
}
