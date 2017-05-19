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
            
            TempData["model"] = organismes;
            return RedirectToAction("Create2");

        }



        // GET: Organismes/Create
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Create2()
        {

            Organismes orga = TempData["model"] as Organismes;
           
            orga.PresidentOrganisme = new PresidentOrganisme();
            orga.Personnel = new Personnel();
            orga.Personnel1 = new Personnel();
            

            //ICollection<Responsable> resps = new Collection<Responsable>();
            //orga.Responsable = resps;


            if (orga == null)
                return RedirectToAction("Create");
         
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

            orga.LigueId = ((Ligues)Session["Ligue"]).Id;
            db.Organismes.Add(orga);
            db.SaveChanges();
            return RedirectToAction("Index");
            

           
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
            if (ModelState.IsValid)
            {
                if (db.Responsable.Any(r => r.Licence == sr.NumeroLicence))
                {
                    ResponsableOrgaViewModel Ro = new ResponsableOrgaViewModel();
                    Ro.responsable = db.Responsable.Where(r => r.Licence == sr.NumeroLicence).First();
                    Ro.OrganismeId = sr.OrganismeId;
                    TempData["responsable"] = Ro;
                    return RedirectToAction("AddResponsable");
                }
                else
                {
                    ModelState.AddModelError("NumeroLicence", "Ne nous trouvons personne avec le numéro de licence : "+sr.NumeroLicence);
                }
            }

            // Tempdata use when delete on responsable is called
            TempData["organisme"] = org.Id;
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
                ro.responsable.Password = encrypt(ro.responsable.Password);
                db.Responsable.Attach(ro.responsable);
                organisme.Responsable.Add(ro.responsable);
                db.SaveChanges();
                return RedirectToAction("ShowResponsable","Organismes",new { @id = ro.OrganismeId });

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
            db.Organismes.Remove(organismes);
            db.Lieux.Remove(organismes.Lieux);
            db.PresidentOrganisme.Remove(organismes.PresidentOrganisme);
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
    }
}
