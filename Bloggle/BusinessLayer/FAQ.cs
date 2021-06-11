using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Bloggle.BusinessLayer
{
    public class FAQ
    {
        public int Id { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string Solution { get; set; }
        public int Vote { get; set; }
    }
}