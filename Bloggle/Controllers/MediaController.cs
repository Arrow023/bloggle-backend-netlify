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
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Configuration;
using System.Management;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Bloggle.Controllers
{
	public class MediaController : ApiController
	{
		public DataAccessService service;
		private Cloudinary _cloudinary;
		
		
		public MediaController()
		{
			service = new DataAccessService();
            _cloudinary = new Cloudinary(service.GetConfiguration("Cloudinary"));
            _cloudinary.Api.Secure = true;
        }
		
		// GET api/<controller>/5
		[AllowAnonymous]
		public async Task<HttpResponseMessage> Get(int id)
		{
			var media = service.FindMedia(id);
			if (media != null)
			{
				HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
				var httpClient = new HttpClient();
				var data = await httpClient.GetAsync(media.Location.ToString());
                byte[] contentBytes = await data.Content.ReadAsByteArrayAsync();
				response.Content = new ByteArrayContent(contentBytes);
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
				string base64File = null;
                if (HttpContext.Current.Request.Files["upload_file"] == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                HttpPostedFile uploadedFile = HttpContext.Current.Request.Files["upload_file"];
                using (var binaryReader = new BinaryReader(uploadedFile.InputStream))
                {
                    byte[] fileBytes = binaryReader.ReadBytes(uploadedFile.ContentLength);
                    string base64String = Convert.ToBase64String(fileBytes);
                    string mimeType = uploadedFile.ContentType;
                    base64File = $"data:{mimeType};base64,{base64String}";
                }
                var uploadParams = new ImageUploadParams()
				{
					File = new FileDescription(base64File),
					UseFilename = true,
					UniqueFilename = false,
					Overwrite = true
				};
                var uploadResult = _cloudinary.Upload(uploadParams);
				var cloudinaryJSON = JsonConvert.DeserializeObject<CloudinaryJSON>(uploadResult.JsonObj.ToString());
				if (uploadResult == null)
				{
					return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Media Not created");
                }

                using (ApplicationDbContext db = new ApplicationDbContext())
				{
					string currentUserId = User.Identity.GetUserId();
					ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
					Medium medium = new Medium();
					
					medium.Type = cloudinaryJSON.Format;
					medium.Location = cloudinaryJSON.SecureUrl;
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