using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.BusinessLayer;
using System.Data.Entity;
namespace Bloggle.DataAcessLayer
{
    public class DataAccessService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BloggleContext context;

        public DataAccessService()
        {
            context = new BloggleContext();
        }

        public Blog CreateBlog(Blog blog)
        {
            try
            {
                context.Blogs.Add(blog);
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return blog;
                else
                    return null;
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return null;
            }
            
        }

        public Blog FindBlog(int blogId)
        {
            return context.Blogs.Find(blogId);
        }

        public List<Blog> GetTrendingBlogs(int take = 12)
        {
            var blogs = context.Blogs.OrderByDescending(b => b.CreatedTime)
                .Take(take)
                .OrderByDescending(b => b.Likes)
                .ToList();
            return blogs;
        }

        public List<Blog> GetPopularBlogs(int take = 12)
        {
            var blogs = context.Blogs
                .OrderByDescending(b => b.Likes)
                .Take(take)
                .ToList();
            return blogs;
        }

        public List<Blog> GetRecentBlogs(int take = 12)
        {
            var blogs = context.Blogs
                .OrderByDescending(b => b.CreatedTime)
                .Take(take)
                .ToList();
            return blogs;
        }

        public List<Blog> GetCategoryXBlogs(int categoryId, int take = 10)
        {
            var blogs = context.Blogs
                .OrderByDescending(b => b.CreatedTime)
                .Where(b => b.CategoryNavigator.Id == categoryId)
                .Take(take)
                .ToList();
            return blogs;
        }

        public Blog UpdateBlog(int blogId, Blog blog)
        {
            try
            {
                var result = context.Blogs.Find(blogId);
                if (result != null)
                {
                    result.Title = blog.Title;
                    result.Subtitle = blog.Subtitle;
                    result.Content = blog.Content;
                    result.MediaId = blog.MediaId;
                    result.LastUpdatedTime = blog.LastUpdatedTime;
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return blog;
                    else
                        return null;
                }
                return null;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return null;
            }
            
        }

        public bool DeleteBlog(int blogId)
        {
            try
            {
                var blog = context.Blogs.Find(blogId);
                if (blog != null)
                {

                    if (blog.MediaId != null)
                    {
                        var medias = context.Media.Where(m => m.Id == blog.MediaId);
                        foreach (var item in medias)
                        {
                            context.Media.Remove(item);
                        }
                    }
                    context.Blogs.Remove(blog);
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return false;
            }
            
        }
        

        public List<Blog> SearchBlogs(int categoryId, string searchValue)
        {
            if (categoryId == 0)
            {
                var blog = context.Blogs
                .Where(b =>
                    (b.Title.ToLower().Contains(searchValue.ToLower())
                    || (b.Subtitle.ToLower().Contains(searchValue.ToLower()))
                    ))
                .ToList();
                return blog;
            }
            else
            {
                var blogs = context.Blogs
                .Where(b => b.CategoryNavigator.Id == categoryId 
                && (b.Title.ToLower().Contains(searchValue.ToLower())
                    || (b.Subtitle.ToLower().Contains(searchValue.ToLower()))
                    ))
                .ToList();

                return blogs;
            }
        }

        public List<string> GetCategories()
        {
            var categories = context.Categories.Select(c => c.CategoryName).ToList();
            return categories;
        }


    }
}