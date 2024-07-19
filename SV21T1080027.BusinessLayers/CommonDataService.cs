using System;
using System.Runtime.CompilerServices;
using SV21T1080027.DataLayers;
using SV21T1080027.DomainModels;

namespace SV21T1080027.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly CustomerDAL customerDB;

        static CommonDataService() {
            string connectionString = @"server=DESKTOP-9VFSRAA; user id=sa; password=123; database=LiteCommerceDB;TrustServerCertificate=true";
            customerDB = new CustomerDAL(connectionString);
        }

        public static List<Customer> ListOfCustomers()
        {
            return customerDB.List();
        }
    }
}
