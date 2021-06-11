using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.Models;
using System.ComponentModel.DataAnnotations;
namespace Bloggle.BusinessLayer
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string CommentValue { get; set; }
        [Required]
        public int BlogId { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }

        public virtual Blog BlogNavigator { get; set; }
        
    }
}