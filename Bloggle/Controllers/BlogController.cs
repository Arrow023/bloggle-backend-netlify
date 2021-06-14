using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bloggle.DataAcessLayer;
using Bloggle.BusinessLayer;
using System.Threading.Tasks;

namespace Bloggle.Controllers
{
    [RoutePrefix("api/Blog")]
    public class BlogController : ApiController
    {
        
        public DataAccessService service;

        public BlogController()
        {
            service = new DataAccessService();
        }


        // GET api/Blog/feature?blogType=<type>
        [AllowAnonymous]
        [Route("feature")]
        public HttpResponseMessage Get(string blogType, int perPage=12)
        {
            if (blogType == "latest")
            {
                var blogs = service.GetRecentBlogs(perPage);
                if (blogs == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No blogs found in latest category");
                else
                    return Request.CreateResponse(HttpStatusCode.OK,blogs);
            }
            else if (blogType == "trending")
            {
                var blogs = service.GetTrendingBlogs(perPage);
                if (blogs == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No blogs found in trending category");
                else
                    return Request.CreateResponse(HttpStatusCode.OK, blogs);
            }
            else
            {
                var blogs = service.GetPopularBlogs(perPage);
                if (blogs == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No blogs found in Popular category");
                else
                    return Request.CreateResponse(HttpStatusCode.OK, blogs);
            }           
        }

        //GET api/Blog/category?category=<category>&perPage=<count>
        [AllowAnonymous]
        [Route("category")]
        public HttpResponseMessage GetCategoricalBlog(int category, int perPage = 12)
        {
            if (category != 0)
            {
                var blogs = service.GetCategoryXBlogs(category, perPage);
                if (blogs == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"No blogs found in {category}");
                else
                    return Request.CreateResponse(HttpStatusCode.OK, blogs);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Please provide category to search");
            }
        }


        // GET api/<controller>/5
        [AllowAnonymous]
        public HttpResponseMessage Get(int id)
        {
            var blog =  service.FindBlog(id);
            if (blog != null)
                return Request.CreateResponse(HttpStatusCode.OK, blog);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,$"Blog with Id={id} not found");
        }

        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ModelState);
            }
            else
            {
                var result = service.CreateBlog(blog);
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, blog);
                }
                else
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Technical Error occurred");
            }
        }

        // PUT api/Blog/5
        public HttpResponseMessage Put(int id, [FromBody]Blog blog)
        {
            if(ModelState.IsValid)
            {
                var result = service.UpdateBlog(id, blog);
                if (result != null)
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented,"Blog is not modified");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Blog/5
        public HttpResponseMessage Delete(int id)
        {
            var status = service.DeleteBlog(id);
            if (status)
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, $"An error occurred during deletion of Blog with Id={id}");
        }
    }
}