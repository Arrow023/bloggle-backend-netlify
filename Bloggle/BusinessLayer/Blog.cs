using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Author { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public int? MediaId { get; set; }
        public int Likes { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }
        [Required]
        public DateTime LastUpdatedTime { get; set; }

        [ForeignKey("MediaId")]
        public virtual Medium MediaNavigator { get; set; }
        public virtual Category CategoryNavigator { get; set; }
        public virtual ICollection<Comment> CommentsNavigator { get; set; }
        
    }
}