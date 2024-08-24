using SV21T1080027.DomainModels;

namespace SV21T1080027.Web.Models
{
    public class OrderDetailModel
    {
        public Order Order { get; set; } = new Order();
        public List<OrderDetail> Details { get; set; } = new List<OrderDetail> ();

        public decimal Total {
            get {
                decimal sum = 0;
                if (Details != null)
                {
                    foreach (var elem in Details)
                    {
                        sum += elem.TotalPrice;
                    }
                }
                return sum;
            }
        }
    }
}
