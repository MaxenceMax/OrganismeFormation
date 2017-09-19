using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrganismeFormation.Controllers
{
    public class CandidatsFormationsController : Controller
    {

        GestionOFEntities db = new GestionOFEntities();

        [Authorize(Roles = "Admin,Responsable")]
        // GET: CandidatsFOrmations
        public ActionResult Index(decimal id )
        {
            CandidatsFormations candidat = db.CandidatsFormations.Find(id);

            return View(candidat);
        }

        public ActionResult EditCandidat(decimal id)
        {
            return View();
        }
    }
}