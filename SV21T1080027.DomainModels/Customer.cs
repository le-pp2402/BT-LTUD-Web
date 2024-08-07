﻿namespace SV21T1080027.DomainModels
{
    public class Customer
    {
        /// <summary>
        /// Thông tin khách hàng    
        /// </summary>
        public int CustomerID { get; set; }
        public String CustomerName { get; set; } = String.Empty;
        public String ContactName { get; set; } = String.Empty;
        public String Province { get; set; } = String.Empty;
        public String Address { get; set; } = String.Empty;
        public String Phone { get; set; } = String.Empty;
        public String Email { get; set; } = String.Empty;
        public bool IsLocked { get; set; }

    }
}
