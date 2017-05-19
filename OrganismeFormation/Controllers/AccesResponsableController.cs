using OrganismeFormation.Models;
using OrganismeFormation.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            var resp = db.Responsable.Find(((Responsable)Session["Responsable"]).Id);
            System.Diagnostics.Debug.WriteLine(resp.Id);
            return View(resp.Organismes);
        }

        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult ParametreResponsable(Responsable responsable)
        {
            if (ModelState.IsValid)
            {
                var updateResp = db.Responsable.Find(responsable.Id);
                db.Responsable.Attach(updateResp);
                if(updateResp.Licence != responsable.Licence)
                {
                    if(db.Responsable.Any(r => r.Licence == responsable.Licence))
                    {
                        ModelState.AddModelError("Licence", "Un responsabe est déjà renseigné avec ce numéro de licence veuillez en choisir un autre.");
                        return View(responsable);
                    }
                    updateResp.Licence = responsable.Licence;
                }
                if (updateResp.Password != responsable.Password)
                {
                    updateResp.Password = encrypt(responsable.Password);
                }

                updateResp.Nom = responsable.Nom;
                updateResp.Prenom = responsable.Prenom;
                updateResp.Telephone = responsable.Telephone;
                updateResp.Email = responsable.Email;
                db.SaveChanges();
                return RedirectToAction("Home");
            }
            return View(responsable);
        }

        private String encrypt(string mdp)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(mdp);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string hashedText = BitConverter.ToString(hashedBytes);
            return hashedText;

        }

        [Authorize(Roles ="Responsable")]
        public ActionResult ParametreResponsable()
        {
            Responsable rp = db.Responsable.Find(((Responsable)Session["Responsable"]).Id);
            return View(rp);
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
            formations.organisme = org;
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 1 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 1 && f.FormationEnded == true);

            return View(formations);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EtatFormationAS(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes", "AccesResponsable");
            Organismes org = db.Organismes.Find(id);
            ListeFormationEtatViewModel formations = new ListeFormationEtatViewModel();
            formations.organisme = org;
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 2 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 2 && f.FormationEnded == true);

            return View(formations);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EtatFormationCFEB(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes", "AccesResponsable");
            Organismes org = db.Organismes.Find(id);
            ListeFormationEtatViewModel formations = new ListeFormationEtatViewModel();
            formations.organisme = org;
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 3 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 3 && f.FormationEnded == true);

            return View(formations);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EtatFormationCQP(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes", "AccesResponsable");
            Organismes org = db.Organismes.Find(id);
            ListeFormationEtatViewModel formations = new ListeFormationEtatViewModel();
            formations.organisme = org;
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 4 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 4 && f.FormationEnded == true);

            return View(formations);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EtatFormationBPJEPS(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes", "AccesResponsable");
            Organismes org = db.Organismes.Find(id);
            ListeFormationEtatViewModel formations = new ListeFormationEtatViewModel();
            formations.organisme = org;
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 5 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 5 && f.FormationEnded == true);

            return View(formations);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EtatFormationDEJEPS(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes", "AccesResponsable");
            Organismes org = db.Organismes.Find(id);
            ListeFormationEtatViewModel formations = new ListeFormationEtatViewModel();
            formations.organisme = org;
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 6 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 6 && f.FormationEnded == true);

            return View(formations);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EtatFormationDESJEPS(decimal id)
        {
            if (id == null)
                RedirectToAction("Organismes", "AccesResponsable");
            Organismes org = db.Organismes.Find(id);
            ListeFormationEtatViewModel formations = new ListeFormationEtatViewModel();
            formations.organisme = org;
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 7 && f.FormationEnded == false);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 7 && f.FormationEnded == true);

            return View(formations);
        }
    }
}