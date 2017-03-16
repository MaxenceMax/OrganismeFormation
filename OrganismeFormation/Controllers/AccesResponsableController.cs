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
            GestionOFEntities db = new GestionOFEntities();
            System.Diagnostics.Debug.WriteLine("ID RESP" + ((Responsable) Session["Responsable"]).Nom);
            Responsable rp = (Responsable)Session["Responsable"];
            return View(rp.Organismes);
        }
    }
}