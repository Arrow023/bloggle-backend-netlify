using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bloggle.DataAcessLayer;
using Bloggle.BusinessLayer;
using Bloggle.Models;
using Microsoft.AspNet.Identity;
namespace Bloggle.Controllers
{
    public class CommentsController : ApiController
    {
        public DataAccessService service;

        public CommentsController()
        {
            service = new DataAccessService();
        }

        //GET api/comments
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            var comments = service.FindAllComments();
            if (comments != null)
                return Request.CreateResponse(HttpStatusCode.OK, comments);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No comments available");
        }

        [HttpGet]
        public HttpResponseMessage FindAllComments(string username)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                var comments = service.FindAllComments(currentUser.UserName);
                if (comments.Count > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, comments);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"No comments found for the user = {currentUser.UserName}");
            }

        }


        // GET api/comments/5
        [AllowAnonymous]
        public HttpResponseMessage Get(int id)
        {
            var comment = service.FindComment(id);
            if (comment != null)
                return Request.CreateResponse(HttpStatusCode.OK, comment);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Comment with id={id} not found");
        }

        

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]Comment comment)
        {
            if (ModelState.IsValid)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    string currentUserId = User.Identity.GetUserId();
                    ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                    comment.CreatedBy = currentUser.UserName;
                    var status = service.AddComment(comment);
                    if (status == ProcessState.Done)
                        return Request.CreateResponse(HttpStatusCode.Created, comment);
                    else if (status == ProcessState.TechnicalError)
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to create a new Category");
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Comment Not created");
                }
                
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }


        // PUT api/comments/5
        public HttpResponseMessage Put(int id, [FromBody]Comment comment)
        {
            if (ModelState.IsValid)
            {                
                var status = service.UpdateComment(id, comment);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.OK, comment);
                else if (status == ProcessState.NotDone)
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Comment not updated");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to update comment");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }


        // DELETE api/comments/5
        public HttpResponseMessage Delete(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string currentUserId = User.Identity.GetUserId();
                bool isAdmin = User.IsInRole("Admin");
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);               
                var status = service.DeleteComment(id,currentUser,isAdmin);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.OK, "Successfully Deleted");
                else if (status == ProcessState.NotDone)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Comment could not be Deleted");
                                  
                }
                else
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "Failed to delete comment");                
            }
        }
    }
}