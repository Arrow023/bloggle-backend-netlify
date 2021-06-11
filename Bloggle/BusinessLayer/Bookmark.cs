using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.Models;
using System.ComponentModel.DataAnnotations;
namespace Bloggle.BusinessLayer
{
    public class Bookmark
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int BlogId { get; set; }

        public virtual Blog BlogNavigator { get; set; }
        
    }
}