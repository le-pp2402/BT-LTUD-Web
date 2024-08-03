﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1080027.BusinessLayers
{
    /// <summary>
    /// Cầu hình cho tầng Business
    /// </summary>
    public static class Configuration
    {
        public static string ConnectionString { get; private set; } = "";
        /// <summary>
        /// khởi tạo cấu hình
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            ConnectionString = connectionString;
        }

    }
}
