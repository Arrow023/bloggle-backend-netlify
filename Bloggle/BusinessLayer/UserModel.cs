using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bloggle.BusinessLayer
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string About { get; set; }
        public string Email { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Facebook { get; set; }
        public int? ProfilePicture { get; set; }

    }
}