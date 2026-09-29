using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using movie_management_system.Models;

namespace movie_management_system.Controllers
{
    public class AccountController : Controller
    {
        string constring = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_RegisterUser", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@User_Name", model.User_Name);
                        cmd.Parameters.AddWithValue("@Email_ID", model.Email_ID);
                        cmd.Parameters.AddWithValue("@User_password", model.User_password);
                        cmd.Parameters.AddWithValue("@City", model.City);
                        cmd.Parameters.AddWithValue("@PhoneNo", model.PhoneNo);

                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            int result = Convert.ToInt32(dr["Result"]);
                            string message = dr["Message"].ToString();

                            if (result == 1)
                            {
                                TempData["SuccessMessage"] = "Registration successful! Please login.";
                                return RedirectToAction("Login");
                            }
                            else
                            {
                                ViewBag.ErrorMessage = message;
                                return View(model);
                            }
                        }
                    }
                }
            }
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_LoginUser", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email_ID", model.Email_ID);
                        cmd.Parameters.AddWithValue("@User_password", model.User_password);

                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            Session["User_ID"] = dr["User_ID"];
                            Session["User_Name"] = dr["User_Name"].ToString();

                            TempData["SuccessMessage"] = "Login successful!";
                            return RedirectToAction("BookMovie", "Booking");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Invalid Email or Password.";
                        }
                    }
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            TempData["SuccessMessage"] = "You have logged out.";
            return RedirectToAction("Login");
        }
    }
}
