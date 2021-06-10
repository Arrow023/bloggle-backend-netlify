using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.Models;

namespace Bloggle.BusinessLayer
{
    public class Bookmark
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int BlogId { get; set; }

        public virtual Blog Blog { get; set; }
        
    }
}