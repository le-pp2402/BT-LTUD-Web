using SV21T1080027.DomainModels;

namespace SV21T1080027.Web.Models
{
    public class OrderDetailModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}
