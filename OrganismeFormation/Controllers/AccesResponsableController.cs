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
            Responsable resp = Session["Responsable"] as Responsable;
            var tmp = resp.ResponsableOrganisme;
            ICollection<Organismes> orga = new Collection<Organismes>();
            foreach(var v in tmp)
            {
                orga.Add(v.Organismes);
            }
            return View(orga);
        }
    }
}