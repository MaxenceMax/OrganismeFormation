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

        [Authorize(Roles = "Responsable")]
        public ActionResult DeleteCandidat(decimal id)
        {
            CandidatsFormations cand = db.CandidatsFormations.Find(id);
            var tmp = cand.FormationId;
            try {
                db.Resultats.Remove(cand.Resultats.FirstOrDefault());
                db.CandidatsFormations.Remove(cand);
                db.SaveChanges();
            }catch (Exception e)
            {

            }

            return RedirectToAction("ShowASAC", "Formations", new { id = tmp });
        }

        public ActionResult EditCandidat(decimal id)
        {
            return View();
        }
    }
}