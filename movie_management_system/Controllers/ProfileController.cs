using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using movie_management_system.Models;

namespace movie_management_system.Controllers
{
    public class ProfileController : Controller
    {
        string constring = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;

        public ActionResult Index()
        {
            int userId = Session["User_ID"] != null ? Convert.ToInt32(Session["User_ID"]) : 1;
            User u = new User();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetUserById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@User_ID", userId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        u.User_ID = Convert.ToInt32(dr["User_ID"]);
                        u.User_Name = dr["User_Name"].ToString();
                        u.Email_ID = dr["Email_ID"].ToString();
                        u.City = dr["City"].ToString();
                        u.PhoneNo = dr["PhoneNo"].ToString();
                        u.User_password = dr["User_password"].ToString();
                        u.ConfirmPassword = dr["User_password"].ToString();
                    }
                }
            }
            return View(u);
        }

        [HttpPost]
        public ActionResult Index(User model)
        {
            int userId = Session["User_ID"] != null ? Convert.ToInt32(Session["User_ID"]) : 1;

            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateUserProfile", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@User_ID", userId);
                        cmd.Parameters.AddWithValue("@User_Name", model.User_Name);
                        cmd.Parameters.AddWithValue("@Email_ID", model.Email_ID);
                        cmd.Parameters.AddWithValue("@City", model.City);
                        cmd.Parameters.AddWithValue("@PhoneNo", model.PhoneNo);
                        cmd.Parameters.AddWithValue("@User_password", model.User_password);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                Session["User_Name"] = model.User_Name;
                TempData["SuccessMessage"] = "Profile Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
