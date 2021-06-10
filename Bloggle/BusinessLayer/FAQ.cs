using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggle.BusinessLayer
{
    public class FAQ
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Solution { get; set; }
        public int Vote { get; set; }
    }
}