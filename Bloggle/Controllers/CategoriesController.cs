using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bloggle.DataAcessLayer;
using Bloggle.BusinessLayer;
namespace Bloggle.Controllers
{

    public class CategoriesController : ApiController
    {
        public DataAccessService service;

        public CategoriesController()
        {
            service = new DataAccessService();
        }

        // GET api/categories
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            var categories = service.GetCategories();
            if (categories != null)
                return Request.CreateResponse(HttpStatusCode.OK, categories);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No categories available");
        }

        // GET api/categories/1
        [AllowAnonymous]
        public HttpResponseMessage Get(int id)
        {
            var category = service.FindCategory(id);
            if (category != null)
                return Request.CreateResponse(HttpStatusCode.OK, category);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Category with id={id} not found");
        }


        // POST api/categories
        public HttpResponseMessage Post([FromBody]Category category)
        {
            if (ModelState.IsValid)
            {
                var status = service.AddCategory(category);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.Created, category);
                else if (status == ProcessState.TechnicalError)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to create a new Category");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented,"Category Not created");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }


        // PUT api/categories/1
        public HttpResponseMessage Put(int id, [FromBody]Category category)
        {
            if (ModelState.IsValid)
            {
                var status = service.UpdateCategory(id, category);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.OK, category);
                else if (status == ProcessState.NotDone)
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Category not updated");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to update category");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            
        }

        // DELETE api/categories/5
        public HttpResponseMessage Delete(int id)
        {
            return Request.CreateErrorResponse(HttpStatusCode.Forbidden,"Deletion of cateogory is not supported");
        }
    }
}