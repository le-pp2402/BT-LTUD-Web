﻿
namespace SV21T1080027.DomainModels
{
    /// <summary>
    /// Loại hàng
    /// </summary>
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
