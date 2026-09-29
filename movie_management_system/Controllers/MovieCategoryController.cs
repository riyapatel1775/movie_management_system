using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using movie_management_system.Models;

namespace movie_management_system.Controllers
{
    public class MovieCategoryController : Controller
    {
        string constring = ConfigurationManager.ConnectionStrings["MovieDBConnection"].ConnectionString;

        public ActionResult Index()
        {
            List<MovieCategory> list = new List<MovieCategory>();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetCategories", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        MovieCategory cat = new MovieCategory();
                        cat.Cat_ID = Convert.ToInt32(dr["Cat_ID"]);
                        cat.Cat_Type = dr["Cat_Type"].ToString();
                        list.Add(cat);
                    }
                }
            }
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(MovieCategory model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Cat_Type", model.Cat_Type);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Category Added Successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            MovieCategory cat = new MovieCategory();

            using (SqlConnection con = new SqlConnection(constring))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetCategoryById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Cat_ID", id);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        cat.Cat_ID = Convert.ToInt32(dr["Cat_ID"]);
                        cat.Cat_Type = dr["Cat_Type"].ToString();
                    }
                }
            }
            return View(cat);
        }

        [HttpPost]
        public ActionResult Edit(MovieCategory model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Cat_ID", model.Cat_ID);
                        cmd.Parameters.AddWithValue("@Cat_Type", model.Cat_Type);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Category Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(constring))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Cat_ID", id);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Category Deleted Successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
