using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace movie_management_system.Models
{
    public class Booking
    {
        [Display(Name = "Booking ID")]
        public int booking_ID { get; set; }

        public int User_ID { get; set; }

        [Display(Name = "User Name")]
        public string User_Name { get; set; }

        [Required(ErrorMessage = "Please select a movie category")]
        [Display(Name = "Movie Category")]
        public int Cat_ID { get; set; }

        [Display(Name = "Category")]
        public string Cat_Type { get; set; }

        [Required(ErrorMessage = "Please select a movie")]
        [Display(Name = "Movie")]
        public int Movie_ID { get; set; }

        [Display(Name = "Movie Name")]
        public string Movie_name { get; set; }

        [Display(Name = "Ticket Rate (₹)")]
        public decimal rate { get; set; }

        [Required(ErrorMessage = "Please enter number of tickets")]
        [Range(1, 10, ErrorMessage = "You can book between 1 and 10 tickets only")]
        [Display(Name = "Number of Tickets")]
        public int no_of_Tickets { get; set; }

        [Display(Name = "Total Amount (₹)")]
        public decimal amount { get; set; }

        // Category dropdown items
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        // Movie dropdown items (cascading)
        public IEnumerable<SelectListItem> MovieList { get; set; }
    }
}
