using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GetFreshFood.Models;

namespace GetFreshFood.Controllers
{
    public class CustomerController : Controller
    {
        string sql_connection_dir = "Server=DESKTOP-URDUFDP\\SQLEXPRESS;" + "Database=GetFreshFood;" + "Integrated Security=true";
        // GET: Customer
        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public JsonResult SignUp(Customer entry_data)
        {

            Customer customer_data = new Customer
            {
                FirstName = entry_data.FirstName,
                LastName = entry_data.LastName,
                password = ""+password_hashing(entry_data.password),
                email = entry_data.email,
                phone = entry_data.phone,
                birthday = entry_data.birthday,
                address = entry_data.address,
            };

            using (SqlConnection connection = new SqlConnection(sql_connection_dir))
            {
                connection.Open();

                bool flag = Check_AlreadyRegistered(customer_data.email);
                
                if(flag)
                { 
                // Add New User to Database
                string sql_query = "('" + customer_data.FirstName + "','" + customer_data.LastName + "','" + customer_data.email + "','" + customer_data.password + "','" + customer_data.phone + "','" + customer_data.address + "','" + customer_data.birthday + "')";
                string cmdText = "insert into Customer(firstname,lastname,email,password,phone,address,birthday) values" + sql_query;
                SqlCommand sql_cmd = new SqlCommand(cmdText, connection);

                try
                {
                   object reply_to_client = new
                     {
                         new_user = "true",
                     };

                    sql_cmd.ExecuteNonQuery();
                    connection.Close();

                    // Create SESSION 
                    Session["Registered_User"] = customer_data.FirstName ;
                    Session["clicked_history"] = new List<string>();
                    Session["Customer_email"] = customer_data.email;

                        return Json(reply_to_client, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                }
                else
                {
                    object reply_to_client = new
                    {
                        new_user = "false",
                    };

                    Debug.WriteLine("********** Already Registered *************");
                    return Json(reply_to_client, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public bool Check_AlreadyRegistered(string mail_acc)
        {
            using (SqlConnection sql_connection = new SqlConnection(sql_connection_dir))
            {
                sql_connection.Open();
                string target_sql_data = "select * from Customer where email='" + mail_acc + "';";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();
                string user_email = "";

                while (reader.Read())
                {
                    user_email = (string)reader["email"];
                }
                sql_connection.Close();
                if (user_email != "")
                {
                    return false;
                }
            }
            return true;
        }

        // SIGN IN
        public ActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignIn(FormCollection sign_in_data)
        {
            var email_val = sign_in_data["log_email_bx"];
            string password_val = sign_in_data["login_passwd_bx"];

            string real_password = "";
            string user_email = "";
            string first_name = "";

            using (SqlConnection sql_connection = new SqlConnection(sql_connection_dir))
            {
                sql_connection.Open();
                string target_sql_data = "select * from Customer where email='" + email_val + "';";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();

                while (reader.Read())
                {
                    real_password = (string)reader["password"];
                    first_name = (string)reader["firstname"];
                    user_email = (string)reader["email"];
                }
                sql_connection.Close();
            }

            if(user_email == email_val)
            {
                if(real_password == password_hashing(password_val).ToString())
                {
                    Session["Registered_User"] = first_name;
                    Session["clicked_history"] = new List<string>();
                    Session["Customer_email"] = user_email;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.singin_err = string.Format("Wrong Password!");
                    return View();
                }
            }
            else
            {
                ViewBag.singin_err = string.Format("Account not found!");
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session["Registered_User"] = null;
            Session["clicked_history"] = null;

            return RedirectToAction("Index", "Home");
        }

        // Generate HashCode for Password
        public int password_hashing(string password_value)
        {
            int hashdata = password_value.GetHashCode();
            return hashdata;
        }

        public ActionResult DetailUsers()
        {
            string user_id = (string)Session["Customer_email"];
            
            using (SqlConnection sql_connection = new SqlConnection(sql_connection_dir))
            {
                sql_connection.Open();
                string target_sql_data = "select * from Customer where email='" + user_id + "';";

                SqlCommand sql_cmd = new SqlCommand(target_sql_data, sql_connection);

                SqlDataReader reader = sql_cmd.ExecuteReader();
              
                while (reader.Read())
                {
                    Customer member_customer = new Customer
                    {
                        FirstName = (string)reader["firstname"],
                        LastName = (string)reader["lastname"],
                        email = (string)reader["email"],
                        phone = (string)reader["phone"],
                        address = (string)reader["address"],
                        birthday = (string)reader["birthday"]
                    };
                    ViewBag.User_Profile = member_customer;
                }
                sql_connection.Close();
            }
            return View();
        }
    }
}