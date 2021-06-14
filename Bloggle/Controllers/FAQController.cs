using Bloggle.BusinessLayer;
using Bloggle.DataAcessLayer;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Bloggle.Controllers
{
    [RoutePrefix("api/faq")]
    public class FAQController : ApiController
    {
        public DataAccessService service;

        public FAQController()
        {
            service = new DataAccessService();
        }

        // GET api/faq
        [AllowAnonymous]
        public HttpResponseMessage Get()
        {
            var FAQs = service.GetFAQs();
            if (FAQs.Count != 0)
                return Request.CreateResponse(HttpStatusCode.OK, FAQs);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No FAQs available.");
        }

        // GET api/faq/5
        [AllowAnonymous]
        public HttpResponseMessage Get(int id)
        {
            var faq = service.FindFAQ(id);
            if (faq != null)
                return Request.CreateResponse(HttpStatusCode.OK, faq);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"No FAQ found with the Id = {id}");
        }

        //GET api/faq/vote?id=<id>&votetype=<Up/Down>
        [HttpGet]
        [Route("vote")]
        public HttpResponseMessage Vote(int id, string votetype)
        {
            var status = service.VotingMeter(id, votetype);
            if (status == ProcessState.Done)
                return Request.CreateResponse(HttpStatusCode.OK, "Voting done successfully");
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Failed to vote");
        }

        // POST api/faq
        [Authorize(Roles ="Admin")]
        public HttpResponseMessage Post([FromBody]FAQ faq)
        {
            if (ModelState.IsValid)
            {
                var status = service.AddFAQ(faq);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.Created, faq);
                else if (status == ProcessState.TechnicalError)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to create a new FAQ");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "FAQ Not created");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // PUT api/faq/5
        [Authorize(Roles ="Admin")]
        public HttpResponseMessage Put(int id, [FromBody]FAQ faq)
        {
            if (ModelState.IsValid)
            {
                var status = service.UpdateFAQ(id, faq);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.OK, faq);
                else if (status == ProcessState.NotDone)
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "FAQ not updated");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to update FAQ");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        



        // DELETE api/<controller>/5
        [Authorize(Roles = "Admin")]
        public HttpResponseMessage Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var status = service.DeleteFAQ(id);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.OK,"FAQ successfully deleted");
                else if (status == ProcessState.NotDone)
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "FAQ not deleted");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to delete FAQ");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }
    }
}