using Microsoft.CodeAnalysis.CSharp.Syntax;
using SV21T1080027.DomainModels;

namespace SV21T1080027.Web.Models
{
    public class PaginationSearchResult<T>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; } = 0;
        public required List<T> Data { get; set; }
        public int PageCount
        {
            get
            {
                if (PageSize == 0) return 1;
                return (RowCount + PageSize - 1) / PageSize;
            }
        }
    }
    public class CategorySearchResult : PaginationSearchResult<Category> { }

    public class CustomerSearchResult : PaginationSearchResult<Customer> { }

    public class EmployeeSearchResult : PaginationSearchResult<Employee> { }

    public class ShipperSearchResult : PaginationSearchResult<Shipper> { }

    public class SupplierSearchResult : PaginationSearchResult<Supplier> { }

    public class ProductSearchResult : PaginationSearchResult<Product> {
        public int CategoryID { get; set; }
        public int SupplierID { get; set; } 
        public int MinPrice { get; set; } 
        public int MaxPrice { get; set; }
        public List<Category> Categories { get; set; } = new List<Category>();  
        public List<Supplier> Suppliers { get; set; } = new List<Supplier>();
    } 
}
