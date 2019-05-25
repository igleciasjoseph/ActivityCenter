using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{
    public class Occasion
    {
        [Key]
        public int OccasionId { get; set; }
        [Required(ErrorMessage = "A Name is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "An date is required!")]
        [NotPast(ErrorMessage = "Please send us your time machine! Date must not be in past!")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Field must not be empty")]
        public TimeSpan Time {get;set;}

        [Required(ErrorMessage = "Field must not be empty")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Field must not be empty")]
        public string DurationType {get;set;}

        [Required(ErrorMessage = "A Description  is required")]
        [MinLength(5, ErrorMessage = "Description must be at least 5 characters long")]
        public string Description { get; set; }


        public User Coordinator { get; set; }
        public int UserID { get; set; }


        public List<Join> Attendees { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}