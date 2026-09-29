using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using movie_management_system.Models;

namespace movie_management_system.Controllers
{
    public class MovieController : Controller
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

        public ActionResult Index()
        {
            List<Movie> list = new List<Movie>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMovies", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Movie m = new Movie();
                        m.Movie_ID = Convert.ToInt32(dr["Movie_ID"]);
                        m.Movie_name = dr["Movie_name"].ToString();
                        m.Release_Date = Convert.ToDateTime(dr["Release_Date"]);
                        m.Cat_ID = Convert.ToInt32(dr["Cat_ID"]);
                        m.Cat_Type = dr["Cat_Type"].ToString();
                        m.rate = Convert.ToDecimal(dr["rate"]);
                        list.Add(m);
                    }
                }
            }
            return View(list);
        }

        public ActionResult Create()
        {
            Movie m = new Movie();
            m.Release_Date = DateTime.Today;
            m.CategoryList = GetCategoryList();
            return View(m);
        }

        [HttpPost]
        public ActionResult Create(Movie model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddMovie", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Movie_name", model.Movie_name);
                        cmd.Parameters.AddWithValue("@Release_Date", model.Release_Date);
                        cmd.Parameters.AddWithValue("@Cat_ID", model.Cat_ID);
                        cmd.Parameters.AddWithValue("@rate", model.rate);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Movie Added Successfully!";
                return RedirectToAction("Index");
            }
            model.CategoryList = GetCategoryList();
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            Movie m = new Movie();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMovieById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Movie_ID", id);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        m.Movie_ID = Convert.ToInt32(dr["Movie_ID"]);
                        m.Movie_name = dr["Movie_name"].ToString();
                        m.Release_Date = Convert.ToDateTime(dr["Release_Date"]);
                        m.Cat_ID = Convert.ToInt32(dr["Cat_ID"]);
                        m.Cat_Type = dr["Cat_Type"].ToString();
                        m.rate = Convert.ToDecimal(dr["rate"]);
                    }
                }
            }
            m.CategoryList = GetCategoryList();
            return View(m);
        }

        [HttpPost]
        public ActionResult Edit(Movie model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMovie", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Movie_ID", model.Movie_ID);
                        cmd.Parameters.AddWithValue("@Movie_name", model.Movie_name);
                        cmd.Parameters.AddWithValue("@Release_Date", model.Release_Date);
                        cmd.Parameters.AddWithValue("@Cat_ID", model.Cat_ID);
                        cmd.Parameters.AddWithValue("@rate", model.rate);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Movie Updated Successfully!";
                return RedirectToAction("Index");
            }
            model.CategoryList = GetCategoryList();
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteMovie", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Movie_ID", id);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        int result = Convert.ToInt32(dr["Result"]);
                        string message = dr["Message"].ToString();

                        if (result == 1)
                            TempData["SuccessMessage"] = message;
                        else
                            TempData["ErrorMessage"] = message;
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Search(int? catId)
        {
            ViewBag.CategoryList = GetCategoryList();
            ViewBag.SelectedCatID = catId;

            List<Movie> list = new List<Movie>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                SqlCommand cmd;

                if (catId.HasValue && catId.Value > 0)
                {
                    cmd = new SqlCommand("sp_GetMoviesByCategory", con);
                    cmd.Parameters.AddWithValue("@Cat_ID", catId.Value);
                }
                else
                {
                    cmd = new SqlCommand("sp_GetMovies", con);
                }

                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Movie m = new Movie();
                    m.Movie_ID = Convert.ToInt32(dr["Movie_ID"]);
                    m.Movie_name = dr["Movie_name"].ToString();
                    m.Release_Date = Convert.ToDateTime(dr["Release_Date"]);
                    m.Cat_ID = Convert.ToInt32(dr["Cat_ID"]);
                    m.Cat_Type = dr["Cat_Type"].ToString();
                    m.rate = Convert.ToDecimal(dr["rate"]);
                    list.Add(m);
                }
            }
            return View(list);
        }
    }
}
