using OrganismeFormation.Models;
using OrganismeFormation.ViewModels;
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

        GestionOFEntities db = new GestionOFEntities();
        // GET: AccesResponsable
        [Authorize(Roles ="Responsable")]
        public ActionResult Home()
        {
            return View();
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult Organismes()
        {
            System.Diagnostics.Debug.WriteLine("ID RESP" + ((Responsable) Session["Responsable"]).Nom);
            Responsable rp = (Responsable)Session["Responsable"];
            return View(rp.Organismes);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult ListeFormations(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes","AccesResponsable");

            var orga = db.Organismes.Find(id);

            ListeFormationsViewModel lf = new ListeFormationsViewModel();
            lf.organisme = orga;
            lf.AC = orga.Formations.Where(f => f.TypedeFormationsId == 1);
            lf.AS = orga.Formations.Where(f => f.TypedeFormationsId == 2);
            lf.CFEB = orga.Formations.Where(f => f.TypedeFormationsId == 3);
            lf.CQP = orga.Formations.Where(f => f.TypedeFormationsId == 4);
            lf.BPJEPS = orga.Formations.Where(f => f.TypedeFormationsId == 5);
            lf.DEJEPS = orga.Formations.Where(f => f.TypedeFormationsId == 6);
            lf.DESJEPS = orga.Formations.Where(f => f.TypedeFormationsId == 7);

            return View(lf);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EtatFormationAC(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes", "AccesResponsable");
            Organismes org = db.Organismes.Find(id);
            ListeFormationEtatViewModel formations = new ListeFormationEtatViewModel();
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 1 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 1 && f.FormationEnded == true);

            return View(formations);
        }
    }
}