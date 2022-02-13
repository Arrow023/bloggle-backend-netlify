using Bloggle.BusinessLayer;
using Bloggle.DataAcessLayer;
using Bloggle.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace Bloggle.Controllers
{
	public class MediaController : ApiController
	{
		public DataAccessService service;
		
		public MediaController()
		{
			service = new DataAccessService();
		}
		
		// GET api/<controller>/5
		[AllowAnonymous]
		public HttpResponseMessage Get(int id)
		{
			var media = service.FindMedia(id);
			if (media != null)
			{
				HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
				var path = HostingEnvironment.MapPath("~/App_Data/");
				response.Content = new StreamContent(new FileStream(path+media.Location, FileMode.Open, FileAccess.Read));
				response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
				response.Content.Headers.ContentDisposition.FileName = media.Location;
				return response;
			}
			else
			{
				return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"File not found with Id={id}");
			}
		}

		// POST api/movies
		public HttpResponseMessage Post()
		{
			try
			{
				using (ApplicationDbContext db = new ApplicationDbContext())
				{
					string currentUserId = User.Identity.GetUserId();
					bool isAdmin = User.IsInRole("Admin");
					ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
					Medium medium = new Medium();
					var httpRequest = HttpContext.Current.Request;
					var mediaType = httpRequest.Form["mediaType"];
					if (httpRequest.Files.Count > 0)
					{
						var postedFile = httpRequest.Files["upload_file"];
						var filePath = HostingEnvironment.MapPath("~/App_Data/" + currentUser.UserName + "__" + postedFile.FileName);
						postedFile.SaveAs(filePath);

						medium.Type = mediaType;
						medium.Location = currentUser.UserName + "__" + postedFile.FileName;
						medium.CreatedBy = currentUser.UserName;
						medium.CreatedTime = DateTime.Now;

						var status = service.AddMedia(medium);
						if (status == ProcessState.Done)
							return Request.CreateResponse(HttpStatusCode.Created, medium);
						else if (status == ProcessState.TechnicalError)
							return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to create a new media");
						else
							return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Media Not created");
					}
					else
					{
						return Request.CreateResponse(HttpStatusCode.BadRequest);
					}
				}
			}
			catch(Exception e)
			{
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError,e.StackTrace);
			}
		}

		
		// DELETE api/media/5
		public HttpResponseMessage Delete(int id)
		{
			//var media = service.FindMedia(id);
			//if (media != null)
			//{
			//    var status = service.DeleteMedia(id);
			//    if (status == ProcessState.Done)
			//    {
			//        string path = HttpContext.Current.Server.MapPath("~/UserData/" + media.Location);
			//        FileInfo file = new FileInfo(path);
			//        if (file.Exists)
			//            file.Delete();
			//        return Request.CreateResponse(HttpStatusCode.OK, "File Deleted successfully.");
			//    }
			//    else if (status == ProcessState.NotDone)
			//        return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "File deletion failed.");
			//    else
			//        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "File could not be deleted");
			//}
			//else
			//    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not available in the database");3
			return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Deletion operation is restricted for security reasons. Please contact administrator.");
		}
	}
}