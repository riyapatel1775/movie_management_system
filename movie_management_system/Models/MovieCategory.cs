using System;
using System.ComponentModel.DataAnnotations;

namespace movie_management_system.Models
{
    public class MovieCategory
    {
        [Display(Name = "Category ID")]
        public int Cat_ID { get; set; }

        [Required(ErrorMessage = "Please enter category name")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [Display(Name = "Category Name")]
        public string Cat_Type { get; set; }
    }
}
