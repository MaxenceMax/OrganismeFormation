using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrganismeFormation.Models;
using OrganismeFormation.ViewModels;
using System.Collections.ObjectModel;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections;

namespace OrganismeFormation.Controllers
{
    public class OrganismesController : Controller
    {
        private GestionOFEntities db = new GestionOFEntities();

        // GET: Organismes
        public ActionResult Index()
        {
           
            return View((db.Ligues.Find(((Ligues)Session["Ligue"]).Id)).Organismes);
        }

        // GET: Organismes/Details/5
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organismes organismes = db.Organismes.Find(id);
            if (organismes == null)
            {
                return HttpNotFound();
            }
            return View(organismes);
        }

        // GET: Organismes/Create
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Create()
        {
            
            Organismes orga = new Organismes();
            Lieux lieux = new Lieux();
            orga.Lieux = lieux;
            return View(orga);
        }

        // POST: Organismes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Create(Organismes organismes)
        {
            if (ModelState.IsValidField("Libelle") && ModelState.IsValidField("NumeroDeclaration") &&
                ModelState.IsValidField("AnneeDeclaration") && ModelState.IsValidField("Lieux.Adresse") &&
                ModelState.IsValidField("Lieux.CodePostal") && ModelState.IsValidField("Lieux.Ville") &&
                ModelState.IsValidField("Lieux.Telephone") && ModelState.IsValidField("Lieux.Email")
                )
            {
                TempData["model"] = organismes;
                return RedirectToAction("Create2");
            }
            return View(organismes);
        }



        // GET: Organismes/Create
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Create2()
        {

            Organismes orga = TempData["model"] as Organismes;
            if (orga == null)
                return RedirectToAction("Create");

            orga.PresidentOrganisme = new PresidentOrganisme();
            orga.Personnel = new Personnel();
            orga.Personnel1 = new Personnel();
           
            
         
            return View(orga);
        }

        // POST: Organismes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Create2(Organismes orga)
        {
            if (ModelState.IsValid)
            {
                orga.LigueId = ((Ligues)Session["Ligue"]).Id;
                db.Organismes.Add(orga);
                db.SaveChanges();
                return RedirectToAction("Index", "Organismes");
            }
            return View(orga);
        }


        [Authorize(Roles = "AccesLigue")]
        public ActionResult ShowResponsable(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SearchResponsableViewModel sr = new SearchResponsableViewModel();
            sr.organisme = db.Organismes.Find(id);
            sr.OrganismeId = id;

            // Tempdata use when delete on responsable is called
            TempData["organisme"] = sr.organisme.Id;
            return View(sr);
        }

        [HttpPost]
        [Authorize(Roles = "AccesLigue")]
        public ActionResult ShowResponsable(SearchResponsableViewModel sr)
        {
            Organismes org = db.Organismes.Find(sr.OrganismeId);
            sr.organisme = org;

            ResponsableOrgaViewModel Ro = new ResponsableOrgaViewModel();
            Ro.responsable.Licence = sr.NumeroLicence;

            // Tempdata use when delete on responsable is called
            TempData["organisme"] = org.Id;

            if (ModelState.IsValid)
            {
                if (!org.Responsable.Any(r => r.Licence == sr.NumeroLicence))
                {
                    if (db.Responsable.Any(r => r.Licence == sr.NumeroLicence))
                    {
                        Ro.responsable = db.Responsable.Where(r => r.Licence == sr.NumeroLicence).First(); ;
                    }
                    else if (IsExistInWebService(Ro.responsable))
                    {
                    }
                    else
                    {
                        ModelState.AddModelError("NumeroLicence", "La personne avec le numéro de licence " + sr.NumeroLicence + " n'existe pas ou est déjà présente pour cette organisme.");
                        return View(sr);
                    }
                    Ro.OrganismeId = sr.OrganismeId;
                    TempData["responsable"] = Ro;
                    return RedirectToAction("AddResponsable");
                }
                else
                {
                    ModelState.AddModelError("NumeroLicence", "La personne avec le numéro de licence " + sr.NumeroLicence + " n'existe pas ou est déjà présente pour cette organisme.");
                    return View(sr);
                }
            }
            return View(sr);
        }

        //GET Organisme ID for Add Responsable
        [Authorize(Roles = "AccesLigue")]
        public ActionResult AddResponsable()
        {
            ResponsableOrgaViewModel ro = TempData["responsable"] as ResponsableOrgaViewModel;
            Organismes organisme = db.Organismes.Find(ro.OrganismeId);
            ViewBag.libelleOrganisme = organisme.Libelle;
            return View(ro);

        }

        [HttpPost]
        [Authorize (Roles ="AccesLigue")]
        public ActionResult AddResponsable(ResponsableOrgaViewModel ro)
        {
            Organismes organisme = db.Organismes.Find(ro.OrganismeId);
            ViewBag.libelleOrganisme = organisme.Libelle;
            if(ModelState.IsValid)
            {
                db.Organismes.Attach(organisme);
                Responsable toUpdate;
                System.Diagnostics.Debug.WriteLine("Nombre d'orgna pour le resp : " + ro.responsable.Id);
                if (ro.responsable.Id > 0)
                {
                    toUpdate = db.Responsable.Find(ro.responsable.Id);
                    db.Responsable.Attach(toUpdate);
                    if (toUpdate.Password != ro.responsable.Password && toUpdate.Password != encrypt(ro.responsable.Password))
                    {
                        toUpdate.Password = encrypt(ro.responsable.Password);
                    }
                }
                else
                {
                    toUpdate = new Responsable();
                    toUpdate.Password = encrypt(ro.responsable.Password);
                }

                toUpdate.Licence = ro.responsable.Licence;
                toUpdate.Nom = ro.responsable.Nom;
                toUpdate.Prenom = ro.responsable.Prenom;
                toUpdate.Email = ro.responsable.Email;
                toUpdate.Telephone = ro.responsable.Telephone;

                organisme.Responsable.Add(toUpdate);
                db.SaveChanges();

                return RedirectToAction("ShowResponsable","Organismes",new { @id = ro.OrganismeId });
            }
            return View(ro);
        }

        [Authorize (Roles ="AccesLigue")]
        public ActionResult UpdateResponsable(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ResponsableOrgaViewModel ro = new ResponsableOrgaViewModel();
            ro.responsable = db.Responsable.Find(id);
            ro.OrganismeId = System.Convert.ToDecimal(TempData["organisme"]);
            return View(ro);
        }

        [Authorize(Roles ="AccesLigue")]
        [HttpPost]
        public ActionResult UpdateResponsable(ResponsableOrgaViewModel ro)
        {
            if(ModelState.IsValid)
            {
                Responsable toUpdate = db.Responsable.Find(ro.responsable.Id);
                db.Responsable.Attach(toUpdate);
                if(toUpdate.Password != ro.responsable.Password && toUpdate.Password!= encrypt( ro.responsable.Password))
                {
                    toUpdate.Password = encrypt(ro.responsable.Password);
                }
                toUpdate.Licence = ro.responsable.Licence;
                toUpdate.Nom = ro.responsable.Nom;
                toUpdate.Prenom = ro.responsable.Prenom;
                toUpdate.Email = ro.responsable.Email;
                toUpdate.Telephone = ro.responsable.Telephone;
                db.SaveChanges();
                return RedirectToAction("ShowResponsable", "Organismes", new { @id = ro.OrganismeId });

            }
            return View(ro);
        }
        

        [Authorize(Roles ="AccesLigue")]
        public ActionResult DeleteResponsable(decimal id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Responsable rp = db.Responsable.Find(id);
            Organismes orga = db.Organismes.Find(TempData["organisme"]);

            db.Organismes.Attach(orga);
            db.Responsable.Attach(rp);

            orga.Responsable.Remove(rp);
            db.SaveChanges();

            System.Diagnostics.Debug.WriteLine("Nombre d'orgna pour le resp : "+ rp.Organismes.Count());
            if (rp.Organismes.Count() == 0)
            {
                db.Responsable.Remove(rp);
            }
            db.SaveChanges();

            return RedirectToAction("ShowResponsable", "Organismes", new { @id = orga.Id });
        }




        // GET: Organismes/Edit/5
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organismes organismes = db.Organismes.Find(id);
            if (organismes == null)
            {
                return HttpNotFound();
            }

            return View(organismes);
        }

        // POST: Organismes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Edit(Organismes organismes)
        {
         
            if (ModelState.IsValid)
            {
                //db.Entry(organismes).State = EntityState.Modified;

                var org = db.Organismes.Find(organismes.Id);
                db.Organismes.Attach(org);
                org.Libelle = organismes.Libelle;
                org.AnneeDeclaration = organismes.AnneeDeclaration;
                org.NumeroDeclaration = organismes.NumeroDeclaration;


                var tmp = db.Lieux.Find(organismes.LieuxId);
                db.Lieux.Attach(tmp);
                tmp.Adresse = organismes.Lieux.Adresse;
                tmp.CodePostal = organismes.Lieux.CodePostal;
                tmp.Ville = organismes.Lieux.Ville;
                tmp.Telephone = organismes.Lieux.Telephone;
                tmp.Email = organismes.Lieux.Email;

                var pres = db.PresidentOrganisme.Find(organismes.PresidentId);
                db.PresidentOrganisme.Attach(pres);
                pres.Nom = organismes.PresidentOrganisme.Nom;
                pres.Prenom = organismes.PresidentOrganisme.Prenom;
                pres.Email = organismes.PresidentOrganisme.Email;
                pres.Telephone = organismes.PresidentOrganisme.Telephone;

                var coor = db.Personnel.Find(organismes.CoordinateurId);
                db.Personnel.Attach(coor);
                coor.Nom = organismes.Personnel.Nom;
                coor.Prenom = organismes.Personnel.Prenom;
                coor.Email = organismes.Personnel.Email;
                coor.Telephone = organismes.Personnel.Telephone;

                var dir = db.Personnel.Find(organismes.DirecteurId);
                db.Personnel.Attach(dir);
                dir.Nom = organismes.Personnel1.Nom;
                dir.Prenom = organismes.Personnel1.Prenom;
                dir.Email = organismes.Personnel1.Email;
                dir.Telephone = organismes.Personnel1.Telephone;


                db.SaveChanges();

                return RedirectToAction("Index");
            }
            

            return View(organismes);
        }

        // GET: Organismes/Delete/5
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organismes organismes = db.Organismes.Find(id);
            if (organismes == null)
            {
                return HttpNotFound();
            }
            return View(organismes);
        }



        // POST: Organismes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AccesLigue")]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Organismes organismes = db.Organismes.Find(id);
            db.Lieux.Remove(organismes.Lieux);
            db.PresidentOrganisme.Remove(organismes.PresidentOrganisme);
            db.Personnel.Remove(organismes.Personnel);
            db.Personnel.Remove(organismes.Personnel1);
            db.Organismes.Remove(organismes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private String encrypt(string mdp)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(mdp);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string hashedText = BitConverter.ToString(hashedBytes);
            return hashedText;

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /** call web service url and check the licence number*/
        private Boolean IsExistInWebService(Models.Responsable model)
        {
            /**
            Create url from licencie
            */
            String url = "http://www.ffjda.org/ws_mobile/webRestGet/service.svc/infosInscriptionASP/";
            String numLicChange = model.Licence.Replace("*", "@").Replace(" ", "§");
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
                    model.Licence = (String)item;
                    // Si le num licence est vide alors la licence n'existe pas
                    if (model.Licence == null || model.Licence.Length == 0)
                        return false;
                    items.TryGetValue("mail", out item);
                    model.Email = (String)item;
                    items.TryGetValue("prenom", out item);
                    model.Prenom = (String)item;
                    items.TryGetValue("nom", out item);
                    model.Nom = (String)item;
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
    }
}
