using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggle.BusinessLayer
{
    public class BookmarkBlog
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Author { get; set; }
        public int? MediaId { get; set; }
        public string Category { get; set; }
        public int Likes { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}