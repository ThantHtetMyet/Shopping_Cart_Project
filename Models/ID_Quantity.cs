using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFreshFood.Models
{
    public class ID_Quantity
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDetails { get; set; }
        public string ProductImagePath { get; set; }
        public string ProductQuantity { get; set; }
        public string ProductPurchasedDate { get; set; }
        public List<string> ProductActivationCode { get; set; }
        public string CustomerId { get; set; }
    }
}