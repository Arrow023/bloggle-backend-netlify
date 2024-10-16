﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Bloggle.Models;
using Bloggle.Providers;
using Bloggle.Results;
using Bloggle.BusinessLayer;
using System.Web.Http.Cors;
using System.Linq;
using Bloggle.DataAcessLayer;
using System.Web.Hosting;
using System.Net;
using System.IO;
using System.Configuration;
using Google.Apis.Auth;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Bloggle.Controllers
{
	[Authorize]
	[RoutePrefix("api/Account")]  
	public class AccountController : ApiController
	{
		private const string LocalLoginProvider = "Local";
		private ApplicationUserManager _userManager;
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private string _googleClientID = new DataAccessService().GetConfiguration("GoogleClientId");
		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager,
			ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
		{
			UserManager = userManager;
			AccessTokenFormat = accessTokenFormat;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		[Route("Allusers")]
		[AllowAnonymous]
		[HttpGet]
		public List<string> GetUsernames()
		{
			using (ApplicationDbContext db = new ApplicationDbContext())
			{
				return db.Users.Select(u => u.UserName).ToList();
			}
		}

		public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

		// GET api/Account/UserInfo
		//[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[AllowAnonymous]
		[Route("UserInfo")]
		public UserModel GetUserInfo(string userName)
		{
			//ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
			UserModel userModel = new UserModel();
			try
			{
				using (ApplicationDbContext db = new ApplicationDbContext())
				{
					//var userName = User.Identity.GetUserName();
					var currentUser = db.Users.FirstOrDefault(u => u.UserName == userName);

					userModel.Id = currentUser.Id;
					userModel.FirstName = currentUser.FirstName;
					userModel.LastName = currentUser.LastName;
					userModel.UserName = currentUser.UserName;
					userModel.Email = currentUser.Email;
					userModel.DOB = currentUser.DOB;
					userModel.PhoneNumber = currentUser.PhoneNumber;
					userModel.About = currentUser.About;
					userModel.Twitter = currentUser.Twitter;
					userModel.Instagram = currentUser.Instagram;
					userModel.Facebook = currentUser.Facebook;
				}

				using (BloggleContext context = new BloggleContext())
				{
					var media = context.Media
						.OrderByDescending(m=>m.CreatedTime)
						.FirstOrDefault(m => m.Type == "dp" && m.CreatedBy == userModel.UserName);
					if (media != null)
						userModel.ProfilePicture = media.Id;
					else
						userModel.ProfilePicture = null;
				}
			}
			catch (Exception e)
			{
				log.Error(e.StackTrace);
				return userModel;
			}
			return userModel;
		}

		// POST api/Account/Logout
		[Route("Logout")]
		public IHttpActionResult Logout()
		{
			Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
			return Ok("Successfully Logged out");
		}

		// GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
		[Route("ManageInfo")]
		public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
		{
			IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				return null;
			}

			List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

			foreach (IdentityUserLogin linkedAccount in user.Logins)
			{
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = linkedAccount.LoginProvider,
					ProviderKey = linkedAccount.ProviderKey
				});
			}

			if (user.PasswordHash != null)
			{
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = LocalLoginProvider,
					ProviderKey = user.UserName,
				});
			}

			return new ManageInfoViewModel
			{
				LocalLoginProvider = LocalLoginProvider,
				Email = user.UserName,
				Logins = logins,
				ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
			};
		}

		//GET LOGS
		[AllowAnonymous]
		[Route("logs")]
		public HttpResponseMessage Get()
		{
			HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
			var path = HostingEnvironment.MapPath("~/logs/");
			response.Content = new StreamContent(new FileStream(path + "Bloggle.log", FileMode.Open, FileAccess.Read));
			response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
			response.Content.Headers.ContentDisposition.FileName = "Bloggle.log";
			return response;
		}

		[AllowAnonymous]
		[Route("loginwithgoogle")]
		public async Task<IHttpActionResult> GetLoginUsingGoogle(string accessToken)
		{
			var settings = new GoogleJsonWebSignature.ValidationSettings
			{
				Audience = new List<string> { _googleClientID }
			};
			var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken, settings);
            if (payload == null)
            {
				return InternalServerError();
            }

			var user = UserManager.FindByEmail(payload.Email);
			if(user == null) //new user
			{
                var newUser = new ApplicationUser()
                {
                    UserName = payload.GivenName,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    DOB = DateTime.Now,
                    Email = payload.Email
                };
				var hashedPassword = GetHashedPassword(payload.GivenName, payload.Email);

                IdentityResult result = await UserManager.CreateAsync(newUser, hashedPassword);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                var addToRoleResult = await UserManager.AddToRoleAsync(newUser.Id, "Blogger");
            }

			return await AutoLogin(payload.GivenName, payload.Email);

		}

		// POST api/Account/ChangePassword
		[Route("ChangePassword")]
		public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
				model.NewPassword);
			
			if (!result.Succeeded)
			{
				return GetErrorResult(result);
			}

			return Ok();
		}

		// POST api/Account/SetPassword
		[Route("SetPassword")]
		public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

			if (!result.Succeeded)
			{
				return GetErrorResult(result);
			}

			return Ok();
		}

		[HttpPut]
		[Route("UpdateUser")]
		public IHttpActionResult UpdateUser(UserModel model)
		{
			if (ModelState.IsValid)
			{
				using (var context = new ApplicationDbContext())
				{
					var store = new UserStore<ApplicationUser>(context);
					var manager = new UserManager<ApplicationUser>(store);
					ApplicationUser user = new ApplicationUser();

					user = manager.FindByEmail(model.Email);
					user.About = model.About;
					user.PhoneNumber = model.PhoneNumber;
					user.Facebook = model.Facebook;
					user.Twitter = model.Twitter;
					user.Instagram = model.Instagram;
					user.ProfilePicture = model.ProfilePicture;
					Task.WaitAny(manager.UpdateAsync(user));
					Task.WaitAny(context.SaveChangesAsync());
					return Ok("Done");
				}   
			}
			else
			{
				return InternalServerError();
			}

		}

		// POST api/Account/AddExternalLogin
		[Route("AddExternalLogin")]
		public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

			AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

			if (ticket == null || ticket.Identity == null || (ticket.Properties != null
				&& ticket.Properties.ExpiresUtc.HasValue
				&& ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
			{
				return BadRequest("External login failure.");
			}

			ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

			if (externalData == null)
			{
				return BadRequest("The external login is already associated with an account.");
			}

			IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
				new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

			if (!result.Succeeded)
			{
				return GetErrorResult(result);
			}

			return Ok();
		}

		// POST api/Account/RemoveLogin
		[Route("RemoveLogin")]
		public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			IdentityResult result;

			if (model.LoginProvider == LocalLoginProvider)
			{
				result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
			}
			else
			{
				result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
					new UserLoginInfo(model.LoginProvider, model.ProviderKey));
			}

			if (!result.Succeeded)
			{
				return GetErrorResult(result);
			}

			return Ok();
		}

		// GET api/Account/ExternalLogin
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
		[AllowAnonymous]
		[Route("ExternalLogin", Name = "ExternalLogin")]
		public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
		{
			if (error != null)
			{
				return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
			}

			if (!User.Identity.IsAuthenticated)
			{
				return new ChallengeResult(provider, this);
			}

			ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

			if (externalLogin == null)
			{
				return InternalServerError();
			}

			if (externalLogin.LoginProvider != provider)
			{
				Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				return new ChallengeResult(provider, this);
			}

			ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
				externalLogin.ProviderKey));

			bool hasRegistered = user != null;

			if (hasRegistered)
			{
				Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				
				 ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
					OAuthDefaults.AuthenticationType);
				ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
					CookieAuthenticationDefaults.AuthenticationType);

				AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
				Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
			}
			else
			{
				IEnumerable<Claim> claims = externalLogin.GetClaims();
				ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
				Authentication.SignIn(identity);
			}

			return Ok();
		}

		// GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
		[AllowAnonymous]
		[Route("ExternalLogins")]
		public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
		{
			IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
			List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

			string state;

			if (generateState)
			{
				const int strengthInBits = 256;
				state = RandomOAuthStateGenerator.Generate(strengthInBits);
			}
			else
			{
				state = null;
			}

			foreach (AuthenticationDescription description in descriptions)
			{
				ExternalLoginViewModel login = new ExternalLoginViewModel
				{
					Name = description.Caption,
					Url = Url.Route("ExternalLogin", new
					{
						provider = description.AuthenticationType,
						response_type = "token",
						client_id = Startup.PublicClientId,
						redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
						state = state
					}),
					State = state
				};
				logins.Add(login);
			}

			return logins;
		}
		// POST api/Account/Register
		[AllowAnonymous]
		[Route("Register")]
		public async Task<IHttpActionResult> Register(RegisterBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var user = new ApplicationUser() {
				UserName = model.UserName,
				FirstName = model.FirstName,
				LastName = model.LastName,
				DOB = model.DOB,
				Email = model.Email };

			IdentityResult result = await UserManager.CreateAsync(user, model.Password);
			
			if (!result.Succeeded)
			{
				return GetErrorResult(result);
			}
			var addToRoleResult = await UserManager.AddToRoleAsync(user.Id, "Blogger");

			return Ok(user.Id);
		}

		// POST api/Account/RegisterExternal
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("RegisterExternal")]
		public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var info = await Authentication.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return InternalServerError();
			}

			var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

			IdentityResult result = await UserManager.CreateAsync(user);
			if (!result.Succeeded)
			{
				return GetErrorResult(result);
			}

			result = await UserManager.AddLoginAsync(user.Id, info.Login);
			if (!result.Succeeded)
			{
				return GetErrorResult(result); 
			}
			return Ok();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _userManager != null)
			{
				_userManager.Dispose();
				_userManager = null;
			}

			base.Dispose(disposing);
		}

		#region Helpers

		private IAuthenticationManager Authentication
		{
			get { return Request.GetOwinContext().Authentication; }
		}

		private IHttpActionResult GetErrorResult(IdentityResult result)
		{
			if (result == null)
			{
				return InternalServerError();
			}

			if (!result.Succeeded)
			{
				if (result.Errors != null)
				{
					foreach (string error in result.Errors)
					{
						ModelState.AddModelError("", error);
					}
				}

				if (ModelState.IsValid)
				{
					// No ModelState errors are available to send, so just return an empty BadRequest.
					return BadRequest();
				}

				return BadRequest(ModelState);
			}

			return null;
		}

		private string GetHashedPassword(string username, string email)
		{
			string result;
			using (MD5 hash = MD5.Create())
			{
				result = String.Join
				(
					"",
					from ba in hash.ComputeHash
					(
						Encoding.UTF8.GetBytes(username + email)
					)
					select ba.ToString("x2")
				);
			}
			
			return result+"A#B";
		}

		private async Task<IHttpActionResult> AutoLogin(string username, string email)
		{
            using (var client = new HttpClient())
            {
                var baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port;
                // Set the base address for the API
                client.BaseAddress = new Uri(baseUrl);

                // Prepare data as key-value pairs
                var formData = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("username", username),
                        new KeyValuePair<string, string>("password", GetHashedPassword(username,email)),
                        new KeyValuePair<string, string>("grant_type", "password")
                    };

                // Encode data as application/x-www-form-urlencoded
                var content = new FormUrlEncodedContent(formData);
                HttpResponseMessage response = await client.PostAsync(baseUrl + "/token", content);

                // Check the response status code
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    var responseData = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseData);
                    return Ok(jsonObject);
                }
                else
                {
                    return InternalServerError();
                }
            }
        }


        private class ExternalLoginData
		{
			public string LoginProvider { get; set; }
			public string ProviderKey { get; set; }
			public string UserName { get; set; }

			public IList<Claim> GetClaims()
			{
				IList<Claim> claims = new List<Claim>();
				claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

				if (UserName != null)
				{
					claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
				}

				return claims;
			}

			public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
			{
				if (identity == null)
				{
					return null;
				}

				Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

				if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
					|| String.IsNullOrEmpty(providerKeyClaim.Value))
				{
					return null;
				}

				if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
				{
					return null;
				}

				return new ExternalLoginData
				{
					LoginProvider = providerKeyClaim.Issuer,
					ProviderKey = providerKeyClaim.Value,
					UserName = identity.FindFirstValue(ClaimTypes.Name)
				};
			}
		}

		private static class RandomOAuthStateGenerator
		{
			private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

			public static string Generate(int strengthInBits)
			{
				const int bitsPerByte = 8;

				if (strengthInBits % bitsPerByte != 0)
				{
					throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
				}

				int strengthInBytes = strengthInBits / bitsPerByte;

				byte[] data = new byte[strengthInBytes];
				_random.GetBytes(data);
				return HttpServerUtility.UrlTokenEncode(data);
			}
		}

		#endregion
	}
}
