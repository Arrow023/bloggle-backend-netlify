using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.Models;
namespace Bloggle.BusinessLayer
{
    public class Medium
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
        
    }
}