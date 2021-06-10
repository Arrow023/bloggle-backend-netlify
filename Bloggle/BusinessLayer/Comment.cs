using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.Models;
namespace Bloggle.BusinessLayer
{
    public class Comment
    {
        public int Id { get; set; }
        public string Comment1 { get; set; }
        public int BlogId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Blog Blog { get; set; }
        
    }
}