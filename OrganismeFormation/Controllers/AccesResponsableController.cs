using OrganismeFormation.Models;
using OrganismeFormation.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OrganismeFormation.Controllers
{
    public class AccesResponsableController : Controller
    {
        [Authorize(Roles = "Responsable")]
        public ActionResult NewCandidatFirstStep(decimal id)
        {
            CandidatTuteurViewModel model = new CandidatTuteurViewModel();
            if (id != null)
            {
                model.FormationId = id;
            }
            else
            {
                RedirectToAction("Home");
            }
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult NewCandidatFirstStep(CandidatTuteurViewModel model)
        {
            if (ModelState.IsValid)
            {
                CandidatFormationViewModel candidat = new CandidatFormationViewModel();
                if(model.LicenceTuteur != null)
                {
                    if(db.Tuteurs.Where(t => t.NumeroLicence == model.LicenceTuteur).Count() ==1)
                    {
                        candidat.Tuteur = db.Tuteurs.Where(t => t.NumeroLicence == model.LicenceTuteur).FirstOrDefault();
                    }
                    else
                    {
                        candidat.Tuteur.NumeroLicence = model.LicenceTuteur;
                        IsExistInWebServiceTuteur(candidat.Tuteur);
                    }
                }
                if(db.CandidatsFormations.Where(c => c.NumeroLicence == model.LicenceCandidat).Count() == 1)
                {
                    candidat.Candidat = db.CandidatsFormations.Where(c => c.NumeroLicence == model.LicenceCandidat).FirstOrDefault();
                }
                else
                {
                    candidat.Candidat.NumeroLicence = model.LicenceCandidat;
                    IsExistInWebServiceCandidat(candidat.Candidat);
                }

                candidat.Formation = db.Formations.Where(f => f.Id == model.FormationId).FirstOrDefault();
                TempData["candidatViewModel"] = candidat;
                return RedirectToAction("NewCandidatSecondStep");
            }
            return View(model);
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult NewCandidatSecondStep()
        {
            CandidatFormationViewModel vm = TempData["candidatViewModel"] as CandidatFormationViewModel;
            if(vm == null)
            {
                RedirectToAction("Home");
            }
            ViewBag.Financements = db.TypedeFinancements.ToList();
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult NewCandidatSecondStep(CandidatFormationViewModel model)
        {
            if(ModelState.IsValidField("Candidat.Nom") && ModelState.IsValidField("Candidat.Prenom") && ModelState.IsValidField("Candidat.Grade") &&
                ModelState.IsValidField("Candidat.NumeroLicence") && ModelState.IsValidField("Candidat.Email")  && ModelState.IsValidField("Candidat.Telephone") &&
                ModelState.IsValidField("Tuteur.Nom") && ModelState.IsValidField("Tuteur.Prenom") && ModelState.IsValidField("Tuteur.Email") &&
                ModelState.IsValidField("Tuteur.NumeroLicence")
                )
            {
                CandidatsFormations candidat = model.Candidat;

                Tuteurs t = model.Tuteur;
                candidat.FormationId = model.Formation.Id;
                candidat.TypedeFinancements = db.TypedeFinancements.Where(td => td .Id ==model.FinancementId).FirstOrDefault();
                candidat.Tuteurs = t;
                Resultats r = new Resultats();
                Passages p = new Passages();
                try
                {
                    if(t!= null && db.Tuteurs.Where(tu => tu.Id == model.Tuteur.Id).Count() == 0)
                    {
                        db.Tuteurs.Add(t);
                    }
                    r.PassagesId = 0;
                    db.CandidatsFormations.Add(candidat);
                    db.SaveChanges();
                    r.CandidatsFormations = candidat;
                    db.Resultats.Add(r);
                    db.SaveChanges();
                }
                catch(Exception e)
                {

                }
                return RedirectToAction("ShowASAC", "Formations", new { id = model.Formation.Id });

            }
            else
            {
                ViewBag.Financements = db.TypedeFinancements.ToList();
            }
            return View(model);
        }

        private Boolean IsExistInWebServiceTuteur(Tuteurs model)
        {
            /**
            Create url from licencie
            */
            String url = "http://www.ffjda.org/ws_mobile/webRestGet/service.svc/infosInscriptionASP/";
            String numLicChange = model.NumeroLicence.Replace("*", "@").Replace(" ", "§");
            /**
            Make the request
            */
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + numLicChange);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    // Reader to open http response
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String back = reader.ReadToEnd();
                    // test le retour de la fonction web
                    // Si j'ai rien le licencie n'existe pas, sinon 
                    if (back.Length == 0 || back == null)
                        return false;

                    // Use dictionnary to reade Json string
                    var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(back);
                    // List to skip first stage
                    ArrayList list = (ArrayList)dict["infosInscriptionASPResult"];
                    // Je récupére tous les items de ma chaine json
                    Dictionary<String, Object> items = (Dictionary<String, Object>)list[0];
                    // Et je traite ceux que je veux
                    object item;
                    items.TryGetValue("numLicence", out item);
                    model.NumeroLicence = (String)item;
                    // Si le num licence est vide alors la licence n'existe pas
                    if (model.NumeroLicence == null || model.NumeroLicence.Length == 0)
                        return false;
                    items.TryGetValue("mail", out item);
                    model.Email = (String)item;
                    items.TryGetValue("prenom", out item);
                    model.Prenom = (String)item;
                    items.TryGetValue("nom", out item);
                    model.Nom = (String)item;
                    return true;
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        private Boolean IsExistInWebServiceCandidat(CandidatsFormations model)
        {
            /**
            Create url from licencie
            */
            String url = "http://www.ffjda.org/ws_mobile/webRestGet/service.svc/infosInscriptionASP/";
            String numLicChange = model.NumeroLicence.Replace("*", "@").Replace(" ", "§");
            /**
            Make the request
            */
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + numLicChange);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    // Reader to open http response
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String back = reader.ReadToEnd();
                    // test le retour de la fonction web
                    // Si j'ai rien le licencie n'existe pas, sinon 
                    if (back.Length == 0 || back == null)
                        return false;

                    // Use dictionnary to reade Json string
                    var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(back);
                    // List to skip first stage
                    ArrayList list = (ArrayList)dict["infosInscriptionASPResult"];
                    // Je récupére tous les items de ma chaine json
                    Dictionary<String, Object> items = (Dictionary<String, Object>)list[0];
                    // Et je traite ceux que je veux
                    object item;
                    items.TryGetValue("numLicence", out item);
                    model.NumeroLicence = (String)item;
                    // Si le num licence est vide alors la licence n'existe pas
                    if (model.NumeroLicence == null || model.NumeroLicence.Length == 0)
                        return false;
                    items.TryGetValue("mail", out item);
                    model.Email = (String)item;
                    items.TryGetValue("prenom", out item);
                    model.Prenom = (String)item;
                    items.TryGetValue("nom", out item);
                    model.Nom = (String)item;
                    items.TryGetValue("tel", out item);
                    
                   
                    items.TryGetValue("sexe", out item);
                    model.Sexes = db.Sexes.Where(s=> s.Code == (String)item).FirstOrDefault();

                    items.TryGetValue("tel", out item);
                    model.Telephone = (String)item;
                    return true;
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }


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
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 1 && f.FormationEnded == false).OrderByDescending(f => f.DateDebut);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 1 && f.FormationEnded == true).OrderByDescending(f => f.DateDebut);
            TempData["model"] = org;

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
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 2 && f.FormationEnded == false).OrderByDescending(f => f.DateDebut);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 2 && f.FormationEnded == true).OrderByDescending(f => f.DateDebut);
            TempData["model"] = org;
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
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 3 && f.FormationEnded == false).OrderByDescending(f => f.DateDebut);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 3 && f.FormationEnded == true).OrderByDescending(f => f.DateDebut);
            TempData["model"] = org;
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
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 4 && f.FormationEnded == false).OrderByDescending(f => f.DateDebut);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 4 && f.FormationEnded == true).OrderByDescending(f => f.DateDebut);
            TempData["model"] = org;
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
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 5 && f.FormationEnded == false).OrderByDescending(f => f.DateDebut);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 5 && f.FormationEnded == true).OrderByDescending(f => f.DateDebut);
            TempData["model"] = org;
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
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 6 && f.FormationEnded == false).OrderByDescending(f => f.DateDebut);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 6 && f.FormationEnded == true).OrderByDescending(f => f.DateDebut);
            TempData["model"] = org;
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
            formations.enCours = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 7 && f.FormationEnded == false).OrderByDescending(f => f.DateDebut);
            formations.termine = db.Formations.Where(f => f.OrganismeId == org.Id && f.TypedeFormationsId == 7 && f.FormationEnded == true).OrderByDescending(f => f.DateDebut);
            TempData["model"] = org;
            return View(formations);
        }
    }
}