using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GetFreshFood.Models
{
    public class Customer
    {
        public string id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string password { get; set; }

        public string confirm_password { get; set; }

        public string email { get; set; }

        public string phone { get; set; }

        public string birthday { get; set; }
        public string address { get; set; }
    }
}