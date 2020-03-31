using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GetFreshFood.Models;


namespace GetFreshFood.Controllers
{
    [RoutePrefix("Home")]
    public class HomeController : Controller
    {
        string sql_con = "Server=DESKTOP-URDUFDP\\SQLEXPRESS;" + "Database=GetFreshFood;" + "Integrated Security=true";
        int total_price = 0;

        // GET: Home
        public ActionResult Index()
        {
            //InsertProductData();

            return RedirectToAction("SearchProduct", "Home");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult SearchProduct(FormCollection form_data)
        {   
            // Clear for Shopping Cart Icon
            Session["View_ShoppingCart"] = null;

            List<Product> product_lis = new List<Product>();

            string target_data = (string)form_data["search_bx"];

            using (SqlConnection sql_connection = new SqlConnection(sql_con))
            {
                sql_connection.Open();

                string target_sql_data = "select * from Product where category like '%" + target_data + "%';";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product pro = new Product()
                    {
                        Id = (int)reader["Id"],
                        name = (string)reader["name"],
                        price = (string)reader["price"],
                        quantity = (string)reader["quantity"],
                        color = (string)reader["color"],
                        short_description = (string)reader["short_description"],
                        image_path = (string)reader["image_path"],
                        category = (string)reader["category"]

                    };

                    product_lis.Add(pro);
                }

                ViewBag.product_lis = product_lis;
                ViewBag.Search_data = target_data;

                if (Session["Registered_User"] != null)
                {
                    ViewBag.Cart_Data_History = Session["clicked_history"];
                }

                sql_connection.Close();
            }
            return View();
        }


        [Route("AddCart")]
        public JsonResult AddCart(Product product)
        {
            // Clear for Shopping Cart Icon
            Session["View_ShoppingCart"] = null;

            object customer_history;

            if (Session["Registered_User"] != null)
            {
                Product temp_product = new Product()
                {
                    Id = product.Id
                };

                List<string> clicked_history = (List<string>)Session["clicked_history"];

                if (clicked_history == null)
                {
                    clicked_history = new List<string>();
                }

                clicked_history.Add(product.Id.ToString());

                Session["clicked_history"] = clicked_history;

                customer_history = new
                {
                    clicked_product_id = clicked_history.Count(),
                };

            }
            else if (Session["Registered_User"] == null)
            {
                customer_history = new
                {
                    clicked_product_id = 0,
                };
            }
            else
            {
                return Json(false);
            }

            return Json(customer_history, JsonRequestBehavior.AllowGet);
        }

        // Insert Data to Database
        public void InsertProductData()
        {

            string[] name = new string[7] { "acer", "macbook", "sony", "asus", "hp", "dell", "microsoft" };
            string[] price = new string[7] { "$2500", "$1500", "$1000", "$3000", "$900", "$5500", "$7500" };
            string[] quantity = new string[7] { "5", "3", "2", "13", "8", "7", "11" };
            string[] short_descption = new string[7] { "Latest", "Old model", "Brand New", "7 Gen", "2018", "2019", "2017" };
            string[] color = new string[7] { "white", "cyan", "black", "yellow", "red", "blue", "pink" };
            string[] image_path = new string[7] { "Image/Product/Laptop/laptop_1.png", "Image/Product/Laptop/laptop_2.png", "Image/Product/Laptop/laptop_3.png", "Image/Product/Laptop/laptop_4.png", "Image/Product/Laptop/laptop_5.png", "Image/Product/Laptop/laptop_6.png", "Image/Product/Laptop/laptop_7.png" };
            string[] category = new string[1] { "laptop" };

            for (int i = 0; i < name.Length; i++)
            {
                using (SqlConnection con = new SqlConnection(sql_con))
                {
                    con.Open();

                    string target_sql = "insert into Product(name,price,quantity,color,short_description,image_path,category) values('" + name[i] + "','" + price[i] + "','" + quantity[i] + "','" + color[i] + "','" + short_descption[i] + "','" + image_path[i] + "','" + category[0] + "')";

                    SqlCommand sql_cmd = new SqlCommand(target_sql, con);

                    try
                    {
                        sql_cmd.ExecuteNonQuery();
                        con.Close();

                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("ERROR");
                    }
                }
            }

        }

        [HttpPost]
        public JsonResult UpdateCart(Product pro)
        {
            // Clear for Shopping Cart Icon
            Session["View_ShoppingCart"] = null;

            object customer_history;

            if (Session["Registered_User"] != null)
            {
                List<string> temp_history_cart = (List<string>)Session["clicked_history"];

                if (temp_history_cart.Count == 0)
                {
                    customer_history = new
                    {
                        clicked_product_id = 0,
                    };
                }
                else
                {
                    customer_history = new
                    {
                        clicked_product_id = temp_history_cart.Count(),
                    };
                }
            }
            else
            {
                customer_history = new
                {
                    clicked_product_id = 11111,
                };
            }
            return Json(customer_history, JsonRequestBehavior.AllowGet);
        }

        [Route("SpecificLinkSearch/{specific_item?}")]
        public ActionResult SpecificLinkSearch(string specific_item)
        {
            // Clear for Shopping Cart Icon
            Session["View_ShoppingCart"] = null;

            List<Product> product_lis = new List<Product>();

            using (SqlConnection sql_connection = new SqlConnection(sql_con))
            {
                sql_connection.Open();

                string target_sql_data = "select * from Product where category like '%" + specific_item + "%';";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product pro = new Product()
                    {
                        Id = (int)reader["Id"],
                        name = (string)reader["name"],
                        price = (string)reader["price"],
                        quantity = (string)reader["quantity"],
                        color = (string)reader["color"],
                        short_description = (string)reader["short_description"],
                        image_path = (string)reader["image_path"],
                        category = (string)reader["category"]

                    };

                    product_lis.Add(pro);
                }

                ViewBag.product_lis = product_lis;
                ViewBag.Search_data = specific_item;

                if (Session["Registered_User"] != null)
                {
                    ViewBag.Cart_Data_History = Session["clicked_history"];
                }

                sql_connection.Close();
            }

            return View();
        }

        public ActionResult Show_Cart_List()
        {

            if (Session["Registered_User"]==null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (AddProductID_Quantity() == null || Session["clicked_history"]==null)
            {
                Session["View_ShoppingCart"] = "empty_product_list";
                ViewBag.OrderedProductId_Quantity = null;
                ViewBag.TotalPrice = 0;
                return View();
            }
            else
            {
                Session["View_ShoppingCart"] = "exit_product_list";
                ViewBag.OrderedProductId_Quantity = AddProductID_Quantity();
                ViewBag.TotalPrice = total_price;
            }
            return View();
            }


        public List<OrderedDetails> AddProductID_Quantity()
        {
            total_price = 0;

            List<string> order_product_lis = (List<string>)Session["clicked_history"];
            
            if(order_product_lis.Count == 0)
             {
                     return null;
             }

             List<OrderedDetails> ordered_product_collection = new List<OrderedDetails>();

            // Ordered Product ID and Quantity
            var query = from x in order_product_lis
                    group x by x into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };

           
                foreach (var q in query)
                {
                using (SqlConnection sql_connection = new SqlConnection(sql_con))
                {
                sql_connection.Open();

                string target_sql_data = "select * from Product where Id=" + q.Value + ";";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {
                    Product pro = new Product()
                    {
                        Id = (int)reader["Id"],
                        name = (string)reader["name"],
                        price = (string)reader["price"],
                        quantity = (string)reader["quantity"],
                        color = (string)reader["color"],
                        short_description = (string)reader["short_description"],
                        image_path = (string)reader["image_path"],
                        category = (string)reader["category"],
                    };

                   OrderedDetails temp_data = new OrderedDetails()
                   {
                            CustomerId = (string)Session["Customer_email"],
                            ProductId = q.Value.ToString(),
                            Ordered_Quantity = q.Count,
                            Ordered_Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day),
                            Product_Details = pro.short_description,
                            Product_Image_Path = pro.image_path,
                            Product_Name = pro.name,
                            Product_Price = Convert.ToInt32(pro.price.Replace("$", String.Empty))
                   };
                    ordered_product_collection.Add(temp_data);
                 }
                    sql_connection.Close();
                }
            }
            foreach (OrderedDetails temp_data in ordered_product_collection)
            {
                total_price = total_price + (temp_data.Ordered_Quantity * temp_data.Product_Price);
            }
            return ordered_product_collection;
        }

        [HttpPost]
        public JsonResult UpdateCartInfo(Update_Quantity_Cart order_details)
        {
            bool add_product = false;

            Update_Quantity_Cart temp_data = new Update_Quantity_Cart()
            {
                ProductId = order_details.ProductId,
                Updated_Quantity = order_details.Updated_Quantity,
                Old_Quantity = order_details.Old_Quantity
            };

            List<string> old_history_lis = (List<string>)Session["clicked_history"];

            // Extract Data from Session
            // Ordered Product ID and Quantity
            var query = from x in old_history_lis
                        group x by x into g
                        let count = g.Count()
                        orderby count descending
                        select new { Value = g.Key, Count = count };

            foreach(var old_history_data in query)
            {
                if(old_history_data.Value == temp_data.ProductId)
                {
                    if(old_history_data.Count < Int32.Parse(temp_data.Updated_Quantity))
                    {
                        add_product = true;
                    }
                    else if(old_history_data.Count > Int32.Parse(temp_data.Updated_Quantity))
                    {
                        add_product = false;
                    }
                }
            }

            // Increase from Zero Quantity
            if(!old_history_lis.Contains(temp_data.ProductId)) 
            {

                Debug.WriteLine("ZERO From HERO");
                List<string> order_product_lis = (List<string>)Session["clicked_history"];

                order_product_lis.Add(temp_data.ProductId);
                Session["clicked_history"] = order_product_lis;

                List<OrderedDetails> Sample_OrderDetails = AddProductID_Quantity();
                int Sample_TotalPrice = total_price;
                object Sample_total_price = new
                {
                    total_price = Sample_TotalPrice
                };
                return Json(Sample_total_price, JsonRequestBehavior.AllowGet);
            }

            int temp_update_productID = Int32.Parse(temp_data.ProductId);

            if (add_product)
            {
                old_history_lis.Add(temp_update_productID.ToString());
                Debug.WriteLine("Add ...");
            }
            else
            {
                old_history_lis.Remove(temp_update_productID.ToString());
                Debug.WriteLine("Remove...");
            }

            // Update session
            Session["clicked_history"] = null;
            Session["clicked_history"] = old_history_lis;

            // Calculate Total Cost
            List<OrderedDetails> temp_orderedDetails = AddProductID_Quantity();
            int temp_totalPrice = total_price;
            object new_total_price = new
            {
                total_price = temp_totalPrice
            };
            return Json(new_total_price,JsonRequestBehavior.AllowGet);
        }

        // Read PurchaseHistory 
        public ActionResult PurchaseHistory()
        {
            string customer_email = (string)Session["Customer_email"];

            //  Read from Purchased Table
            if(Session["Customer_email"]!=null)
            {
            List<PurchasedProduct> purchased_products_lis = new List<PurchasedProduct>();
            List<ID_Quantity> id_quantity_lis = new List<ID_Quantity>();
            

            using (SqlConnection sql_connection = new SqlConnection(sql_con))
            {
                
                sql_connection.Open();

                string target_sql_data = "select * from PurchasedProduct where CustomerId='"+ customer_email+"';";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {

                    PurchasedProduct one_purchase_pro = new PurchasedProduct()
                    {
                        Id = (int)reader["Id"],
                        ProductId = (string)reader["ProductId"],
                        ProductName = (string)reader["ProductName"],
                        ProductDetails = (string) reader["ProductDetails"],
                        ProductImagePath = (string) reader["ProductImagePath"],
                        ProductPurchasedDate = (string) reader["ProductPurchasedDate"],
                        ProductActivationCode = (string) reader["ProductActivationCode"],
                        CustomerId = customer_email
                    };

                    purchased_products_lis.Add(one_purchase_pro);
                }
                sql_connection.Close();
            }
 
            var groupedResult = purchased_products_lis.GroupBy(s => s.ProductId);

            bool flager = true;

            foreach (var ageGroup in groupedResult)
            {
                    List<string> activation_code_lis = new List<string>();

                    ID_Quantity temp_id_quantity = new ID_Quantity();
                    temp_id_quantity.ProductId = ageGroup.Key;
                    temp_id_quantity.ProductQuantity = ageGroup.Count().ToString();
                    
                    foreach (PurchasedProduct s in ageGroup)  //Each group has a inner collection  
                    {
                        if(flager)
                        {
                            temp_id_quantity.ProductName = s.ProductName;
                            temp_id_quantity.ProductDetails = s.ProductDetails;
                            temp_id_quantity.ProductImagePath = s.ProductImagePath;
                            temp_id_quantity.ProductPurchasedDate = s.ProductPurchasedDate;
                            flager = false;
                        }
                        activation_code_lis.Add(s.ProductActivationCode);

                    }
                    temp_id_quantity.ProductActivationCode = activation_code_lis;

                    flager = true;
                    id_quantity_lis.Add(temp_id_quantity);
                    activation_code_lis = null;
             }
                ViewBag.Purchased_View = "Yes";
                ViewBag.Purchased_Products_List = id_quantity_lis;
            }
            else
            {
                return RedirectToAction("SearchProduct");
            }
            return View();
        }

        // Clicked CheckOut Link
        // Buy Product
        public ActionResult BuyProduct()
        {
            List<string> purchase_lis = (List<string>)Session["clicked_history"];
            List<PurchasedProduct> purchased_product_lis = new List<PurchasedProduct>();
            string format = "dd MMMM yyyy";
            string current_date = DateTime.Today.ToString(format);

            if (purchase_lis.Count != 0)
            { 
            foreach (string pro_id in purchase_lis)
            {
                using (SqlConnection sql_connection = new SqlConnection(sql_con))
                {
                    sql_connection.Open();

                    string target_sql_data = "select * from Product where Id=" + pro_id + ";";

                    SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                    SqlDataReader reader = sql_cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Product pro = new Product()
                        {
                            Id = (int)reader["Id"],
                            name = (string)reader["name"],
                            price = (string)reader["price"],
                            quantity = (string)reader["quantity"],
                            color = (string)reader["color"],
                            short_description = (string)reader["short_description"],
                            image_path = (string)reader["image_path"],
                            category = (string)reader["category"],
                        };

                       Debug.WriteLine(current_date);

                       PurchasedProduct temp_data = new PurchasedProduct()
                       {
                                ProductImagePath = pro.image_path,
                                ProductId = pro.Id.ToString(),
                                ProductName = pro.name,
                                ProductDetails = pro.short_description,
                                ProductActivationCode = Guid.NewGuid().ToString(),
                                ProductPurchasedDate = current_date
                        };

                        purchased_product_lis.Add(temp_data);
                    }
                    sql_connection.Close();
                }
            }
                Add_to_ProductTable(purchased_product_lis);
               
                return RedirectToAction("PurchaseHistory");
            }
            else
            {
                return Content("<h3>Empty List to Write</h3>");
            }
        }

        public void Add_to_ProductTable(List<PurchasedProduct> purchased_data)
        {
           string customer_email = (string)Session["Customer_email"];

           foreach (PurchasedProduct pro_data in purchased_data)
           {
                using (SqlConnection sql_connection = new SqlConnection(sql_con))
                {
                    sql_connection.Open();

                    string target_query = "('" + pro_data.ProductId + "','" + pro_data.ProductName + "','" + pro_data.ProductDetails + "','" + pro_data.ProductPurchasedDate + "','" + pro_data.ProductActivationCode + "','" + pro_data.ProductImagePath + "','" + customer_email + "')";

                    string cmdText = "insert into PurchasedProduct(ProductId,ProductName,ProductDetails,ProductPurchasedDate,ProductActivationCode,ProductImagePath,CustomerId) values" + target_query;

                    SqlCommand sql_cmd = new SqlCommand(cmdText, sql_connection);

                    try
                    {
                        sql_cmd.ExecuteNonQuery();
                        sql_connection.Close();
                        Session["clicked_history"] = null;
                        Session["clicked_history"] = new List<string>();
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Fail to write to Purchased table.");
                    }
                }
            }
        }


        }
    }
