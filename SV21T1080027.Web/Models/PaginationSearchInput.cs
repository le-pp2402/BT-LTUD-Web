using SV21T1080027.DomainModels;

namespace SV21T1080027.Web.Models
{
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";
    }

    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    } 
}
