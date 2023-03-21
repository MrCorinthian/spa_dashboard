using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace WebApplication13.Controllers.Mobile
{
    [Authorize]
    public class BaseApiController : ApiController
    {
        [NonAction]
        public String GetCurrentUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var context = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            try
            {
                return context.Value;
            }
            catch
            {
                return "";
            }
        }
    }
}
