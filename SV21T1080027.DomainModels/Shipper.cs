namespace SV21T1080027.DomainModels
{
    /// <summary>
    /// Người giao hàng
    /// </summary>
    public class Shipper
    {
        public int ShipperID { get; set; }
        public string ShipperName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
