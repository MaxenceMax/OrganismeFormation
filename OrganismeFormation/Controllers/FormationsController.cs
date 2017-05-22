using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrganismeFormation.Controllers
{
    public class FormationsController : Controller
    {

        // GET: Formations
        public ActionResult Index()
        {
            return View();
        }
    }
}