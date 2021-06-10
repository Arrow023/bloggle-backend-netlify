using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Bloggle.Models;


namespace Bloggle.BusinessLayer
{
    public class Blog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public int Category { get; set; }
        public string Author { get; set; }
        public int? Media { get; set; }
        public int Likes { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }

        public virtual Category Category1 { get; set; }
        public virtual Medium Medium { get; set; }
        public virtual ICollection<Bookmark> Bookmarks { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        
    }
}