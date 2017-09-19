using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OrganismeFormation.Controllers
{
    public class FormationsController : Controller
    {

        GestionOFEntities db = new GestionOFEntities();

        // GET: Formations
        [Authorize(Roles = "Responsable")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Responsable")]
        public ActionResult AddSimpleFormation(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFormations type = db.TypedeFormations.Find(id);
            ViewBag.Libelle = type.Libelle;

            Formations formation = initializeFormation(id);

            return View(formation);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EditSimpleFormation(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Formations formation = db.Formations.Find(id);

            return View(formation);
        }

        [Authorize(Roles ="Responsable")]
        public ActionResult DeleteFormation(decimal id)
        {
            Formations f = db.Formations.Find(id);
            var orga = f.OrganismeId;
            int typeFo = (int) f.TypedeFormationsId;
            string method = "";
    
            db.DescriptifUC.RemoveRange(f.DescriptifUC);
            db.CandidatsFormations.RemoveRange(f.CandidatsFormations);


            

            db.Formations.Remove(f);
            switch (typeFo)
            {
                case 1:
                    method = "EtatFormationAC";
                    break;
                case 2:
                    method = "EtatFormationAS";
                    break;
                case 3:
                    method = "EtatFormationCFEB";
                    break;
                case 4:
                    method = "EtatFormationCQP";
                    break;
                case 5:
                    method = "EtatFormationBPJEPS";
                    break;
                case 6:
                    method = "EtatFormationDEJEPS";
                    break;
                default:
                    method = "EtatFormationDESJEPS";
                    break;

            }
            try
            {
                db.SaveChanges();
            }catch(Exception e)
            {

            }

            return RedirectToAction(method, "AccesResponsable", new { id = orga });
        }


        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult AddSimpleFormation(Formations formation)
        {
            if (ModelState.IsValidField("DateDebut") && ModelState.IsValidField("DateFin") && ModelState.IsValidField("DateLimiteInscription") && ModelState.IsValidField("DateTestSelection") &&
               ModelState.IsValidField("NombreTotalHeures") && ModelState.IsValidField("NbreHeureCentre") && ModelState.IsValidField("NbrHeureStructAccueil") &&
               ModelState.IsValidField("TotalHeureFOAD") && ModelState.IsValidField("HeureELearning") && ModelState.IsValidField("TotalAutresHeures") &&
               ModelState.IsValidField("TypeHeureContenu") && ModelState.IsValidField("CoutComplet") && ModelState.IsValidField("NbreHeureAutreEspace") &&
               ModelState.IsValidField("Personnel.Nom") && ModelState.IsValidField("Personnel.Prenom") && ModelState.IsValidField("Personnel.Email") && ModelState.IsValidField("Personnel.Telephone") &&
               ModelState.IsValidField("Lieux.Ville") && ModelState.IsValidField("Lieux.Adresse") && ModelState.IsValidField("Lieux.CodePostal") &&
               ModelState.IsValidField("Habilitations.NumeroHabilitation") && ModelState.IsValidField("Habilitations.DebutDateDelivrance") && ModelState.IsValidField("Habilitations.FinDateDelivrance") && ModelState.IsValidField("Habilitations.NumeroSession") && ModelState.IsValidField("Habilitations.DateEPMSP") && ModelState.IsValidField("Habilitations.DateTEP")
               )
            {
                // Mise en place de l'organisateur pédagogique
                Personnel p = new Personnel();
                p.Nom = formation.Personnel.Nom;
                p.Prenom = formation.Personnel.Prenom;
                p.Email = formation.Personnel.Email;
                p.Telephone = formation.Personnel.Telephone;

                Lieux l = new Lieux();
                l.Adresse = formation.Lieux.Adresse;
                l.CodePostal = formation.Lieux.CodePostal;
                l.Ville = formation.Lieux.Ville;
                l.Telephone = formation.Lieux.Telephone;

                formation.LieuSiNonPorteuse = l.Id;
                formation.IdOrganisateurpeda = formation.Personnel.Id;
                formation.FormationEnded = false;

                db.Lieux.Add(l);
                db.Personnel.Add(p);
                db.Formations.Add(formation);
                if (formation.Habilitations != null)
                {
                    db.Habilitations.Add(formation.Habilitations);
                }
                db.SaveChanges();
                return backToGoodFormation((decimal)formation.OrganismeId, (decimal)formation.TypedeFormationsId);
            }
            TypedeFormations type = db.TypedeFormations.Find(formation.TypedeFormationsId);
            ViewBag.Libelle = type.Libelle;
            return View(formation);
        }

        private ActionResult backToGoodFormation(decimal org, decimal id)
        {
            List<string> Etats = new List<string>();
            Etats.Add("EtatFormationAC");
            Etats.Add("EtatFormationAS");
            Etats.Add("EtatFormationCFEB");
            Etats.Add("EtatFormationCQP");
            Etats.Add("EtatFormationBPJEPS");
            Etats.Add("EtatFormationDEJEPS");
            Etats.Add("EtatFormationDESJEPS");
            //if (id == 1)
            //{
            return RedirectToAction(Etats[int.Parse(id.ToString()) - 1], "AccesResponsable", new { id = org });
            //}

            //return RedirectToAction("Index");
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult ShowASAC(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formations formation = db.Formations.Find(id);

            return View(formation);
        }



        private Formations initializeFormation(decimal id)
        {

            // recupération de l'organisme auquel sera rattaché notre nouvelle formation
            Organismes orga = TempData["model"] as Organismes;
            // récupération du responsable pour le préremplir comme responsable pédagogique
            Responsable rp = db.Responsable.Find(((Responsable)Session["Responsable"]).Id);

            Formations formation = new Formations();
            formation.TypedeFormationsId = id;
            formation.OrganismeId = orga.Id;

            formation.Habilitations = new Habilitations();

            Personnel pers = new Personnel();
            pers.Nom = rp.Nom;
            pers.Prenom = rp.Prenom;
            pers.Email = rp.Email;
            pers.Telephone = rp.Telephone;

            formation.Personnel = pers;

            return formation;
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult AddSimpleUC(decimal id)
        {
            var descriptif = new DescriptifUC();

            //var formation = db.Formations.Find(id);

            descriptif.FormationsId = id;
            return View(descriptif);
        }

        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult AddSimpleUC(DescriptifUC d)
        {
            if (ModelState.IsValidField("Libelle") && ModelState.IsValidField("ResultatMax"))
            {
                db.DescriptifUC.Add(d);
                db.SaveChanges();
            }
            return RedirectToAction("ShowASAC", "Formations", new { id = d.FormationsId });
        }

        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult EditSimpleFormation(Formations formation)
        {
            if (ModelState.IsValidField("DateDebut") && ModelState.IsValidField("DateFin") && ModelState.IsValidField("DateLimiteInscription") && ModelState.IsValidField("DateTestSelection") &&
               ModelState.IsValidField("NombreTotalHeures") && ModelState.IsValidField("NbreHeureCentre") && ModelState.IsValidField("NbrHeureStructAccueil") &&
               ModelState.IsValidField("TotalHeureFOAD") && ModelState.IsValidField("HeureELearning") && ModelState.IsValidField("TotalAutresHeures") &&
               ModelState.IsValidField("TypeHeureContenu") && ModelState.IsValidField("CoutComplet") && ModelState.IsValidField("NbreHeureAutreEspace") &&
               ModelState.IsValidField("Personnel.Nom") && ModelState.IsValidField("Personnel.Prenom") && ModelState.IsValidField("Personnel.Email") && ModelState.IsValidField("Personnel.Telephone") &&
               ModelState.IsValidField("Lieux.Ville") && ModelState.IsValidField("Lieux.Adresse") && ModelState.IsValidField("Lieux.CodePostal") &&
               ModelState.IsValidField("Habilitations.NumeroHabilitation") && ModelState.IsValidField("Habilitations.DebutDateDelivrance") && ModelState.IsValidField("Habilitations.FinDateDelivrance") && ModelState.IsValidField("Habilitations.NumeroSession") && ModelState.IsValidField("Habilitations.DateEPMSP") && ModelState.IsValidField("Habilitations.DateTEP")
               )
            {

                db.Entry(formation).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ShowASAC", "Formations", new { id = (decimal)formation.Id });
            }
            return View(formation);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult EndSimpleFormation(decimal id)
        {
            var form = db.Formations.Find(id);
            form.FormationEnded = true;

            db.Entry(form).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            return backToGoodFormation((decimal)form.OrganismeId, (decimal)form.TypedeFormationsId);
        }

        public ActionResult ExportCSV(decimal id)
        {
            Formations obj = db.Formations.Find(id);
            StringBuilder sb = new StringBuilder();
            StringBuilder sbP = new StringBuilder();
            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();


            for (int index = 0; index < pi.Length; index++)
            {
                if (pi[index].PropertyType.IsAssignableFrom(typeof(string)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(DateTime)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(int)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(decimal)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(bool)))
                {

                    sb.Append(pi[index].GetValue(obj, null));
                    sbP.Append(pi[index].Name);

                    if (index < pi.Length - 1)
                    {
                        sb.Append(";");
                        sbP.Append(";");
                    }
                }
            }
            return Content((sbP.ToString() + Environment.NewLine + sb.ToString()));
        }
        
        [Authorize(Roles = "Responsable")]
        public ActionResult SaisieNotesCandidatUC(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CandidatsFormations cf = db.CandidatsFormations.Find(id);
            if (cf.Resultats == null || cf.Resultats.Count == 0)
            {
                var res = new Resultats();
                cf.Resultats.Add(res);
                db.Entry(res).State = System.Data.Entity.EntityState.Added;
               // db.SaveChanges();
            }

            foreach (DescriptifUC uc in cf.Formations.DescriptifUC)
            {
                ResultatUc existing = null;
                foreach (ResultatUc resUc in cf.Resultats.FirstOrDefault().ResultatUc)
                {
                    if (resUc.DescriptifUC == uc)
                    {
                        existing = resUc;break;
                    }
                }
                if (existing == null)
                {
                    existing = new ResultatUc();
                    existing.DescriptifUC = uc;
                    cf.Resultats.FirstOrDefault().ResultatUc.Add(existing);
                }
            }

            db.Entry(cf).State = System.Data.Entity.EntityState.Modified;
            return View(cf);
        }

        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult SaisieNotesCandidatUC(CandidatsFormations cf)
        {
            bool isValid = true;
            foreach (string key in ModelState.Keys)
            {
                isValid = isValid && ModelState.IsValidField(key);

            }
            if (isValid)
            {
                db.Entry(cf).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ShowASAC", "Formations", new { id = (decimal)cf.FormationId });
            }
            return View(cf);
        }

    }
}
