﻿using SV21T1080027.DomainModels;

namespace SV21T1080027.Web.Models
{
    public class ProductDetail
    {
        public Product Product { get; set; } = new Product();
        public List<ProductAttribute> Attributes { get; set; } =  new List<ProductAttribute>();
        public List<ProductPhoto> Photos { get; set; } = new List<ProductPhoto>(); 
    }
}
