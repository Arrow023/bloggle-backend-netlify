using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Bloggle.BusinessLayer;
namespace Bloggle.DataAcessLayer
{
    public class BloggleContext : DbContext
    {
        public BloggleContext() : base("BloggleContext")
        {

        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Bookmark> Bookmarks { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<FAQ> FAQs { get; set; }
        public virtual DbSet<Medium> Media { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }

    }
}