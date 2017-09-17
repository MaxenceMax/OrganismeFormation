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

        [AllowAnonymous]
        public ActionResult NewCandidatFirstStep(decimal id)
        {
            CandidatTuteurViewModel model = new CandidatTuteurViewModel();
            model.FormationId = id;
            return View(model);
        }

        public ActionResult NewCandidatFirstStep(CandidatTuteurViewModel model)
        {

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

        private Boolean IsExistInWebServiceCandidat(Candidats model)
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
                    //model.Sexe = (String)item;
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