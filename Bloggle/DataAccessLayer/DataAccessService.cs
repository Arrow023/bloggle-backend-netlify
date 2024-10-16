﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bloggle.BusinessLayer;
using System.Data.Entity;
using Bloggle.Models;
using System.IO;
namespace Bloggle.DataAcessLayer
{
    public enum ProcessState
    {
        Done, NotDone, TechnicalError
    }
    public class DataAccessService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BloggleContext context;

        public DataAccessService()
        {
            context = new BloggleContext();            
        }

        public string GetConfiguration(string name)
        {
            return context.Configurations.FirstOrDefault(s => s.Name == name).Value;
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

        public List<CarouselBlog> GetAllBlogs()
        {
            return context.Blogs
                .Select(b => new CarouselBlog
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Author = b.Author,
                    MediaId = b.MediaId,
                    Category = b.CategoryNavigator.CategoryName,
                    Likes = b.Likes,
                    CreatedTime = b.CreatedTime,
                    LastUpdatedTime = b.LastUpdatedTime,
                    Views = b.Views
                }).ToList();
        }

        public List<CarouselBlog> GetTrendingBlogs(int take = 12)
        {
            var blogs = context.Blogs.OrderByDescending(b => b.CreatedTime)
                .Take(take)
                .OrderByDescending(b => b.Likes)
                .Select(b => new CarouselBlog
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Author = b.Author,
                    MediaId = b.MediaId,
                    Category = b.CategoryNavigator.CategoryName,
                    Likes = b.Likes,
                    CreatedTime = b.CreatedTime,
                    LastUpdatedTime = b.LastUpdatedTime,
                    Views = b.Views
                }).ToList();
            return blogs;
        }

        public List<CarouselBlog> GetPopularBlogs(int take = 12)
        {
            var blogs = context.Blogs
                .OrderByDescending(b => b.Likes)
                .Take(take)
                .Select(b => new CarouselBlog
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Author = b.Author,
                    MediaId = b.MediaId,
                    Category = b.CategoryNavigator.CategoryName,
                    Likes = b.Likes,
                    CreatedTime = b.CreatedTime,
                    LastUpdatedTime = b.LastUpdatedTime,
                    Views = b.Views
                }).ToList();
            return blogs;
        }

        public List<CarouselBlog> GetRecentBlogs(int take = 12)
        {
            var blogs = context.Blogs
                .OrderByDescending(b => b.CreatedTime)
                .Take(take)
                .Select(b => new CarouselBlog
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Author = b.Author,
                    MediaId = b.MediaId,
                    Category = b.CategoryNavigator.CategoryName,
                    Likes = b.Likes,
                    CreatedTime = b.CreatedTime,
                    LastUpdatedTime = b.LastUpdatedTime,
                    Views = b.Views
                }).ToList();
            return blogs;
        }

        public List<CarouselBlog> GetCategoryXBlogs(int categoryId, int take = 10)
        {
            var blogs = context.Blogs
                .OrderByDescending(b => b.CreatedTime)
                .Where(b => b.CategoryNavigator.Id == categoryId)
                .Take(take)
                .Select(b => new CarouselBlog
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Author = b.Author,
                    MediaId = b.MediaId,
                    Category = b.CategoryNavigator.CategoryName,
                    Likes = b.Likes,
                    CreatedTime = b.CreatedTime,
                    LastUpdatedTime = b.LastUpdatedTime,
                    Views = b.Views
                }).ToList();
            return blogs;
        }

        public List<CarouselBlog> GetBlogsOfUser(string userName)
        {
            var blogs = context.Blogs
                .OrderByDescending(b => b.CreatedTime)
                .Where(b => b.Author == userName)
                .Select(b => new CarouselBlog
                {
                    Id = b.Id,
                    Title = b.Title,
                    Subtitle = b.Subtitle,
                    Author = b.Author,
                    MediaId = b.MediaId,
                    Category = b.CategoryNavigator.CategoryName,
                    Likes = b.Likes,
                    CreatedTime = b.CreatedTime,
                    LastUpdatedTime = b.LastUpdatedTime,
                    Views = b.Views
                }).ToList();
            return blogs;
        }

        public ProcessState GiveLike(int blogId)
        {
            try
            {
                var blog = context.Blogs.Find(blogId);
                if (blog != null)
                {
                    blog.Likes = blog.Likes + 1;
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState AddViews(int blogId)
        {
            try
            {
                var result = context.Blogs.Find(blogId);
                if (result != null)
                {
                    result.Views = result.Views + 1;
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return ProcessState.TechnicalError;
            }

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

        public bool DeleteBlog(int blogId, string path)
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
                            var currentfile = path+item.Location;
                            File.Delete(currentfile);
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


        // Data Access for Categories Controller Begins

        public List<Category> GetCategories()
        {
            var categories = context.Categories.ToList();
            return categories;
        }

        public Category FindCategory(int id)
        {
            var category = context.Categories.Find(id);
            return category;
        }

        public ProcessState AddCategory(Category category)
        {
            try
            {
                context.Categories.Add(category);
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return ProcessState.Done;
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
            
        }

        public ProcessState UpdateCategory(int id, Category category)
        {
            var _category = context.Categories.Find(id);
            try
            {
                if (_category != null)
                {
                    _category.CategoryName = category.CategoryName;
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
            
        }

        public List<Medium> GetAllMedia()
        {
            return context.Media.ToList();
        }

        // Data Access for Categories Controller Begins
        public List<Comment> FindAllComments()
        {
            var comments = context.Comments.ToList();
            return comments;
        }

        public Comment FindComment(int commentId)
        {
            var comment = context.Comments.Find(commentId);
            return comment;
        }

        public List<Comment> FindAllComments(string username)
        {
            var comments = context.Comments.Where(c => c.CreatedBy == username).ToList();
            return comments;
        }

        public ProcessState AddComment(Comment comment)
        {
            try
            {
                
                context.Comments.Add(comment);
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return ProcessState.Done;
                else
                    return ProcessState.NotDone;                 
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState UpdateComment(int commentId, Comment comment)
        {
            try
            {
                var _comment = context.Comments.Find(commentId);
                if (_comment != null)
                {
                    _comment.CommentValue = comment.CommentValue;
                    _comment.CreatedTime = comment.CreatedTime;
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;

            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState DeleteComment(int commentId,ApplicationUser user, bool isAdmin)
        {
            try
            {
                var comment = context.Comments.Find(commentId);
                if (comment != null)
                {
                    if (isAdmin || (user.UserName == comment.CreatedBy) || user.UserName == comment.BlogNavigator.Author)
                    {
                        context.Comments.Remove(comment);
                        int rowsAffected = context.SaveChanges();
                        if (rowsAffected > 0)
                            return ProcessState.Done;
                        else
                            return ProcessState.NotDone;
                    }
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
            
        }


        // Data Access for Media Controller Begins

        public Medium FindMedia(int mediaId)
        {
            var media = context.Media.Find(mediaId);
            return media;
        }

        public ProcessState AddMedia(Medium media)
        {
            try
            {
                context.Media.Add(media);
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return ProcessState.Done;
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState DeleteMedia(int mediaId)
        {
            try
            {
                var media = context.Media.Find(mediaId);
                if (media != null)
                {
                    context.Media.Remove(media);
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }


        // Data Access for Media Controller Begins

        public List<FAQ> GetFAQs()
        {
            return context.FAQs.ToList();
        }

        public FAQ FindFAQ(int faqId)
        {
            var faq = context.FAQs.Find(faqId);
            return faq;
        }

        public ProcessState AddFAQ(FAQ faq)
        {
            try
            {
                context.FAQs.Add(faq);
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return ProcessState.Done;
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState VotingMeter(int id, string votetype)
        {
            var faq = context.FAQs.Find(id);
            if (faq != null)
            {
                if (votetype == "Up")
                    faq.Vote += 1;
                else if (votetype == "Down")
                    faq.Vote -= 1;
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return ProcessState.Done;
                else
                    return ProcessState.NotDone;
            }
            else
                return ProcessState.NotDone;
        }

        public ProcessState UpdateFAQ(int faqId, FAQ faq)
        {
            try
            {
                var _faq = context.FAQs.Find(faqId);
                if (_faq != null)
                {
                    _faq.Question = faq.Question;
                    _faq.Solution = faq.Solution;
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState DeleteFAQ(int faqId)
        {
            try
            {
                var faq = context.FAQs.Find(faqId);
                if (faq != null)
                {
                    context.FAQs.Remove(faq);
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        // Data Access for Bookmark controller Begins here

        public List<BookmarkBlog> GetBookmarks(string userName)
        {
            var bookmarks = context.Bookmarks
                .Where(b => b.UserId == userName)
                .Select(b => new BookmarkBlog
                {
                    Id = b.Id,
                    BlogId = b.BlogId,
                    Title = b.BlogNavigator.Title,
                    Subtitle = b.BlogNavigator.Subtitle,
                    Author = b.BlogNavigator.Author,
                    MediaId = b.BlogNavigator.MediaId,
                    Category = b.BlogNavigator.CategoryNavigator.CategoryName,
                    Likes = b.BlogNavigator.Likes,
                    CreatedTime = b.BlogNavigator.CreatedTime,
                    LastUpdatedTime = b.BlogNavigator.LastUpdatedTime
                }).ToList();
            return bookmarks;
        }

        public ProcessState AddBookmark(Bookmark bookmark)
        {
            try
            {
                var _bookmark = context.Bookmarks
                    .Where(b => b.BlogId == bookmark.BlogId && b.UserId == bookmark.UserId)
                    .FirstOrDefault();
                if (_bookmark != null)
                    return ProcessState.Done;

                context.Bookmarks.Add(bookmark);
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return ProcessState.Done;
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState RemoveBookmark(int bookmarkId,string userName)
        {
            try
            {
                var bookmark = context.Bookmarks.Find(bookmarkId);
                if (bookmark != null && bookmark.UserId == userName)
                {
                    context.Bookmarks.Remove(bookmark);
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }



        //Reports Section
        public List<Report> GetReports()
        {
            return context.Reports.ToList();
        }

        public ProcessState AddReport(Report report)
        {
            try
            {
                var reportInDb = context.Reports.Where(r => r.ContentId == report.ContentId
                                                        && r.ReportType == report.ReportType
                                                        && r.ContentType == report.ContentType).FirstOrDefault();
                if (reportInDb != null)
                {
                    reportInDb.ReportedCount += 1;
                }
                else
                    context.Reports.Add(report);
                int rowsAffected = context.SaveChanges();
                if (rowsAffected > 0)
                    return ProcessState.Done;
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }

        public ProcessState DeleteReport(int reportId)
        {
            try
            {
                var report = context.Reports.Find(reportId);
                if (report != null)
                {
                    context.Reports.Remove(report);
                    int rowsAffected = context.SaveChanges();
                    if (rowsAffected > 0)
                        return ProcessState.Done;
                    else
                        return ProcessState.NotDone;
                }
                else
                    return ProcessState.NotDone;
            }
            catch (Exception e)
            {
                log.Error(e.StackTrace);
                return ProcessState.TechnicalError;
            }
        }


    }
}