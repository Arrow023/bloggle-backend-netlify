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
    [Authorize(Roles = "Admin")]
    public class ReportController : ApiController
    {
        public DataAccessService service;

        public ReportController()
        {
            service = new DataAccessService();
        }

        public HttpResponseMessage Get()
        {
            var Reports = service.GetReports();
            if (Reports.Count != 0)
                return Request.CreateResponse(HttpStatusCode.OK, Reports);
            else
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No Reports available.");
        }
        [AllowAnonymous]
        public HttpResponseMessage Post([FromBody] Report report)
        {
            if (ModelState.IsValid)
            {
                var status = service.AddReport(report);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.Created, report);
                else if (status == ProcessState.TechnicalError)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to create a new report");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Report Not created");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        public HttpResponseMessage Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var status = service.DeleteReport(id);
                if (status == ProcessState.Done)
                    return Request.CreateResponse(HttpStatusCode.OK, "Report successfully deleted");
                else if (status == ProcessState.NotDone)
                    return Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "Report not deleted");
                else
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to delete Report");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }



    }
}