using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.Models;
using System.ComponentModel.DataAnnotations;
namespace Bloggle.BusinessLayer
{
    public class Medium
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }
      
    }
}