using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication13.DAL;

namespace WebApplication13.Controllers
{
    public class PrivacyController : Controller
    {
        // GET: Privacy
        public ActionResult Index()
        {
            string privacy = DataDAL.GetMobileSetting("PRIVACY_POLICY");
            return View((object)privacy);
        }
    }
}