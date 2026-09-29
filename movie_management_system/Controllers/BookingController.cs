using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using movie_management_system.Models;

namespace movie_management_system.Controllers
{
    public class BookingController : Controller
    {
        string constring = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;

        private List<SelectListItem> GetCategoryList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetCategories", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new SelectListItem
                        {
                            Value = dr["Cat_ID"].ToString(),
                            Text = dr["Cat_Type"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public JsonResult GetMoviesByCategory(int catId)
        {
            List<object> list = new List<object>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMoviesByCategory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Cat_ID", catId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new
                        {
                            Value = dr["Movie_ID"].ToString(),
                            Text = dr["Movie_name"].ToString(),
                            Rate = Convert.ToDecimal(dr["rate"])
                        });
                    }
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BookMovie()
        {
            if (Session["User_ID"] == null)
            {
                Session["User_ID"] = 1;
            }

            Booking b = new Booking();
            b.no_of_Tickets = 1;
            b.CategoryList = GetCategoryList();
            b.MovieList = new List<SelectListItem>();
            return View(b);
        }

        public ActionResult Create()
        {
            return RedirectToAction("BookMovie");
        }

        [HttpPost]
        public ActionResult BookMovie(Booking model)
        {
            int userId = Session["User_ID"] != null ? Convert.ToInt32(Session["User_ID"]) : 1;

            if (ModelState.IsValid)
            {
                decimal rate = 0;
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetMovieRate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Movie_ID", model.Movie_ID);
                        con.Open();
                        object res = cmd.ExecuteScalar();
                        if (res != null)
                        {
                            rate = Convert.ToDecimal(res);
                        }
                    }
                }

                decimal calculatedAmount = rate * model.no_of_Tickets;

                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddBooking", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@User_ID", userId);
                        cmd.Parameters.AddWithValue("@Cat_ID", model.Cat_ID);
                        cmd.Parameters.AddWithValue("@Movie_ID", model.Movie_ID);
                        cmd.Parameters.AddWithValue("@no_of_Tickets", model.no_of_Tickets);
                        cmd.Parameters.AddWithValue("@amount", calculatedAmount);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Movie Booked Successfully! Total Amount: Rs. " + calculatedAmount;
                return RedirectToAction("MyBookings");
            }

            model.CategoryList = GetCategoryList();
            model.MovieList = new List<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Booking model)
        {
            return BookMovie(model);
        }

        public ActionResult MyBookings()
        {
            int userId = Session["User_ID"] != null ? Convert.ToInt32(Session["User_ID"]) : 1;
            List<Booking> list = new List<Booking>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetBookingsByUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@User_ID", userId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Booking b = new Booking();
                        b.booking_ID = Convert.ToInt32(dr["booking_ID"]);
                        b.User_ID = Convert.ToInt32(dr["User_ID"]);
                        b.Movie_ID = Convert.ToInt32(dr["Movie_ID"]);
                        b.Movie_name = dr["Movie_name"].ToString();
                        b.Cat_ID = Convert.ToInt32(dr["Cat_ID"]);
                        b.Cat_Type = dr["Cat_Type"].ToString();
                        b.rate = Convert.ToDecimal(dr["rate"]);
                        b.no_of_Tickets = Convert.ToInt32(dr["no_of_Tickets"]);
                        b.amount = Convert.ToDecimal(dr["amount"]);
                        list.Add(b);
                    }
                }
            }
            return View(list);
        }

        public ActionResult Edit(int id)
        {
            int userId = Session["User_ID"] != null ? Convert.ToInt32(Session["User_ID"]) : 1;
            Booking b = new Booking();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetBookingById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@booking_ID", id);
                    cmd.Parameters.AddWithValue("@User_ID", userId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        b.booking_ID = Convert.ToInt32(dr["booking_ID"]);
                        b.User_ID = Convert.ToInt32(dr["User_ID"]);
                        b.Movie_ID = Convert.ToInt32(dr["Movie_ID"]);
                        b.Movie_name = dr["Movie_name"].ToString();
                        b.Cat_ID = Convert.ToInt32(dr["Cat_ID"]);
                        b.Cat_Type = dr["Cat_Type"].ToString();
                        b.rate = Convert.ToDecimal(dr["rate"]);
                        b.no_of_Tickets = Convert.ToInt32(dr["no_of_Tickets"]);
                        b.amount = Convert.ToDecimal(dr["amount"]);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
            }
            return View(b);
        }

        [HttpPost]
        public ActionResult Edit(Booking model)
        {
            int userId = Session["User_ID"] != null ? Convert.ToInt32(Session["User_ID"]) : 1;

            if (ModelState.IsValid)
            {
                decimal rate = 0;
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetMovieRate", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Movie_ID", model.Movie_ID);
                        con.Open();
                        rate = Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                }

                decimal newAmount = rate * model.no_of_Tickets;

                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateBooking", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@booking_ID", model.booking_ID);
                        cmd.Parameters.AddWithValue("@User_ID", userId);
                        cmd.Parameters.AddWithValue("@no_of_Tickets", model.no_of_Tickets);
                        cmd.Parameters.AddWithValue("@amount", newAmount);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Booking Updated Successfully! New Amount: Rs. " + newAmount;
                return RedirectToAction("MyBookings");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            int userId = Session["User_ID"] != null ? Convert.ToInt32(Session["User_ID"]) : 1;

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteBooking", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@booking_ID", id);
                    cmd.Parameters.AddWithValue("@User_ID", userId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Booking Cancelled Successfully!";
            return RedirectToAction("MyBookings");
        }
    }
}
