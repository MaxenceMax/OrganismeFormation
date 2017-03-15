using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrganismeFormation.Controllers
{
    public class AccesLiguesController : Controller
    {
        // GET: AccesLigues
        [Authorize(Roles ="AccesLigue")]
        public ActionResult Home()
        {
            return View();
        }
    }
}