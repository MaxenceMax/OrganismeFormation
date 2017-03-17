using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrganismeFormation.Models;
using System.Collections.ObjectModel;

namespace OrganismeFormation.Controllers
{
    public class OrganismesController : Controller
    {
        private GestionOFEntities db = new GestionOFEntities();

        // GET: Organismes
        public ActionResult Index()
        {
            var organismes = db.Organismes.Include(o => o.Lieux).Include(o => o.Ligues).Include(o => o.Personnel).Include(o => o.Personnel1).Include(o => o.PresidentOrganisme);
            return View(organismes.ToList());
        }

        // GET: Organismes/Details/5
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
        public ActionResult Create(Organismes organismes)
        {
            
            TempData["model"] = organismes;
            return RedirectToAction("Create2");

        }



        // GET: Organismes/Create
        public ActionResult Create2()
        {

            Organismes orga = TempData["model"] as Organismes;
           
            orga.PresidentOrganisme = new PresidentOrganisme();
            orga.Personnel = new Personnel();
            orga.Personnel1 = new Personnel();

            ICollection<Responsable> resps = new Collection<Responsable>();
            orga.Responsable = resps;
          
           
            if (orga == null)
                return RedirectToAction("Create");
         
            return View(orga);
        }

        // POST: Organismes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(Organismes orga)
        {

            
            db.Organismes.Add(orga);
            db.SaveChanges();
            return RedirectToAction("Index");
            

           
        }


        public ActionResult AddResponsable()
        {

            //int tempUniqueID = -1 * new Random().Next();

            return PartialView("AddResponsable", new Responsable {});

        }

        // GET: Organismes/Edit/5
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
            ViewBag.LieuxId = new SelectList(db.Lieux, "Id", "Adresse", organismes.LieuxId);
            ViewBag.LigueId = new SelectList(db.Ligues, "Id", "Libelle", organismes.LigueId);
            ViewBag.CoordinateurId = new SelectList(db.Personnel, "Id", "Nom", organismes.CoordinateurId);
            ViewBag.DirecteurId = new SelectList(db.Personnel, "Id", "Nom", organismes.DirecteurId);
            ViewBag.PresidentId = new SelectList(db.PresidentOrganisme, "Id", "Telephone", organismes.PresidentId);
            return View(organismes);
        }

        // POST: Organismes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Libelle,NumeroDeclaration,AnneeDeclaration,LieuxId,PresidentId,CoordinateurId,DirecteurId,ResponsableId,LigueId")] Organismes organismes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(organismes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LieuxId = new SelectList(db.Lieux, "Id", "Adresse", organismes.LieuxId);
            ViewBag.LigueId = new SelectList(db.Ligues, "Id", "Libelle", organismes.LigueId);
            ViewBag.CoordinateurId = new SelectList(db.Personnel, "Id", "Nom", organismes.CoordinateurId);
            ViewBag.DirecteurId = new SelectList(db.Personnel, "Id", "Nom", organismes.DirecteurId);
            ViewBag.PresidentId = new SelectList(db.PresidentOrganisme, "Id", "Telephone", organismes.PresidentId);
            return View(organismes);
        }

        // GET: Organismes/Delete/5
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
        public ActionResult DeleteConfirmed(decimal id)
        {
            Organismes organismes = db.Organismes.Find(id);
            db.Organismes.Remove(organismes);
            db.SaveChanges();
            return RedirectToAction("Index");
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
