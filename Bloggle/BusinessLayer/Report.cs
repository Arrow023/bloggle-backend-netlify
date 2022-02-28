using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bloggle.Models;
using System.ComponentModel;

namespace Bloggle.BusinessLayer
{
    public class Report
    {
        public int Id { get; set; }
        public string ContentType { get; set; }
        public int ContentId { get; set; }
        public string ReportType { get; set; }
        public int? ReportedCount { get; set; }

    }
}