namespace SV21T1080027.DomainModels
{
    public class Product
    {
        public int ProductID { get; set; } = 0;
        public string ProductName { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }
        public string Unit { get; set; } = "";
        public decimal Price { get; set; } = 0;
        public string Photo { get; set; } = "";
        public bool IsSelling { get; set; }
    }
}
