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

        [Authorize(Roles ="Responsable,Admin")]
        public ActionResult EditCandidat(decimal id)
        {
            CandidatsFormations candidat = db.CandidatsFormations.Find(id);
            ViewBag.Financements = db.TypedeFinancements.ToList();
            return View(candidat);
        }

        [Authorize(Roles = "Responsable,Admin")]
        [HttpPost]
        public ActionResult EditCandidat(CandidatsFormations model)
        {
            CandidatsFormations candidat = db.CandidatsFormations.Find(model.Id);
            ViewBag.Financements = db.TypedeFinancements.ToList();
            if (ModelState.IsValidField("Nom") && ModelState.IsValidField("Prenom") && ModelState.IsValidField("Grade") &&
                ModelState.IsValidField("NumeroLicence") && ModelState.IsValidField("Email") && ModelState.IsValidField("Telephone") &&
                ModelState.IsValidField("Tuteurs.Nom") && ModelState.IsValidField("Tuteurs.Prenom") && ModelState.IsValidField("Tuteurs.Email") &&
                ModelState.IsValidField("Tuteurs.NumeroLicence")
                )
            {
                db.CandidatsFormations.Attach(candidat);
                candidat.Nom = model.Nom;
                candidat.Prenom = model.Prenom;
                candidat.NumeroLicence = model.NumeroLicence;
                candidat.Grade = model.Grade;
                candidat.Email = model.Email;
                candidat.StructureAccueil = model.StructureAccueil;
                candidat.TypedeFinancementsId = model.TypedeFinancementsId;
                candidat.DetailsFinancement = model.DetailsFinancement;


                Tuteurs t = db.Tuteurs.Find(candidat.TuteursId);
                db.Tuteurs.Attach(t);
                t.Email = model.Tuteurs.Email;
                t.Nom = model.Tuteurs.Nom;
                t.Prenom = model.Tuteurs.Prenom;
                t.NumeroLicence = model.Tuteurs.NumeroLicence;

                db.SaveChanges();
                return RedirectToAction("Index", new { id = candidat.Id });
            }
            return View(candidat);
        }
    }
}