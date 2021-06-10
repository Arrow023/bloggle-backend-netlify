using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.BusinessLayer;
namespace Bloggle.DataAcessLayer
{
    public class DataAccessService
    {
        public BloggleContext context;
        public DataAccessService()
        {
            context = new BloggleContext();
        }

        public Blog CreateBlog(Blog blog)
        {
            context.Blogs.Add(blog);
            int rowsAffected = context.SaveChanges();
            if (rowsAffected > 0)
                return blog;
            else
                return null;
        }
    }
}