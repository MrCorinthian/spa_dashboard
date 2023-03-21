using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using WebApplication13.Models;
using WebApplication13.Models.Mobile;
using System.Web.Http.Cors;

namespace WebApplication13.Controllers.Mobile
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> SignIn(UserSignin data)
        {
            User user = new User();

            using (var context = new spasystemdbEntities())
            {
                var query = context.Users.FirstOrDefault(c => c.Username == data.Username && c.Password == data.Password);
                if (query != null) user = query;
            }

            return Ok(user);
        }
    }
}
