using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace movie_management_system.Models
{
    public class Movie
    {
        [Display(Name = "Movie ID")]
        public int Movie_ID { get; set; }

        [Required(ErrorMessage = "Please enter movie name")]
        [StringLength(200, ErrorMessage = "Movie name cannot exceed 200 characters")]
        [Display(Name = "Movie Name")]
        public string Movie_name { get; set; }

        [Required(ErrorMessage = "Please select release date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Release Date")]
        public DateTime Release_Date { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int Cat_ID { get; set; }

        [Display(Name = "Category Name")]
        public string Cat_Type { get; set; }

        [Required(ErrorMessage = "Please enter ticket rate")]
        [Range(1.0, 10000.0, ErrorMessage = "Rate must be greater than 0")]
        [Display(Name = "Ticket Rate (₹)")]
        public decimal rate { get; set; }

        // Used for Category DropDownList in Views
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
