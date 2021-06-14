using Bloggle.DataAcessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bloggle.Models;
using Microsoft.AspNet.Identity;
using Bloggle.BusinessLayer;

namespace Bloggle.Controllers
{
    public class BookmarkController : ApiController
    {
        public DataAccessService service;

        public BookmarkController()
        {
            service = new DataAccessService();
        }

        // GET api/bookmark
        public HttpResponseMessage Get()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var userName = currentUser.UserName;
                var bookmarks = service.GetBookmarks(userName);
                if (bookmarks.Count != 0)
                    return Request.CreateResponse(HttpStatusCode.OK, bookmarks);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"No bookmarks available for the user {userName}");
            }
        }


        // POST api/bookmark
        public HttpResponseMessage Post([FromBody]Bookmark bookmark)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var userName = currentUser.UserName;
                bookmark.UserId = userName;
                var status = service.AddBookmark(bookmark);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.Created, "Bookmark added successfully");
                else if (status == ProcessState.TechnicalError)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to add a new bookmark");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Bookmark not added");
            }
        }


        // DELETE api/bookmark/1
        public HttpResponseMessage Delete(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var userName = currentUser.UserName;
                var status = service.RemoveBookmark(id, userName);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.Created, "Bookmark removed successfully");
                else if (status == ProcessState.TechnicalError)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to remove bookmark");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Bookmark not removed");
            }
        }
    }
}