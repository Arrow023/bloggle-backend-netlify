using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggle.BusinessLayer
{
    public class Category
    {
        public int Id { get; set; }
        public string Category1 { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
    }
}