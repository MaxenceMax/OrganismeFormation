using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrganismeFormation.Controllers
{
    public class AccesResponsableController : Controller
    {
        // GET: AccesResponsable
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Organismes()
        {
            return View((Session["Responsable"] as Responsable).Organismes);
        }
    }
}