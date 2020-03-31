using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GetFreshFood.Models
{
    public class Update_Quantity_Cart
    {
        public string ProductId { get; set; }
        public string Old_Quantity { get; set; }
        public string Updated_Quantity { get; set; }
    }
}