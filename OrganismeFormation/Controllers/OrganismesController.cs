using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrganismeFormation.Models;

namespace OrganismeFormation.Controllers
{
    public class OrganismesController : Controller
    {
        private GestionOFEntities db = new GestionOFEntities();

        // GET: Organismes
        [Authorize (Roles ="AccesLigue")]
        public ActionResult Index()
        {
            var organismes = db.Organismes.Include(o => o.Lieux).Include(o => o.Ligues).Include(o => o.Personnel).Include(o => o.Personnel1).Include(o => o.PresidentOrganisme).Include(o => o.Responsable);
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
            ViewBag.LieuxId = new SelectList(db.Lieux, "Id", "Adresse");
            ViewBag.LigueId = new SelectList(db.Ligues, "Id", "Libelle");
            ViewBag.CoordinateurId = new SelectList(db.Personnel, "Id", "Nom");
            ViewBag.DirecteurId = new SelectList(db.Personnel, "Id", "Nom");
            ViewBag.PresidentId = new SelectList(db.PresidentOrganisme, "Id", "Telephone");
            ViewBag.ResponsableId = new SelectList(db.Responsable, "Id", "Nom");
            return View();
        }

        // POST: Organismes/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "AccesLigue")]
        public ActionResult Create([Bind(Include = "Id,Libelle,NumeroDeclaration,AnneeDeclaration,LieuxId,PresidentId,CoordinateurId,DirecteurId,ResponsableId,LigueId")] Organismes organismes)
        {
            if (ModelState.IsValid)
            {
                db.Organismes.Add(organismes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LieuxId = new SelectList(db.Lieux, "Id", "Adresse", organismes.LieuxId);
            ViewBag.LigueId = new SelectList(db.Ligues, "Id", "Libelle", organismes.LigueId);
            ViewBag.CoordinateurId = new SelectList(db.Personnel, "Id", "Nom", organismes.CoordinateurId);
            ViewBag.DirecteurId = new SelectList(db.Personnel, "Id", "Nom", organismes.DirecteurId);
            ViewBag.PresidentId = new SelectList(db.PresidentOrganisme, "Id", "Telephone", organismes.PresidentId);
            ViewBag.ResponsableId = new SelectList(db.Responsable, "Id", "Nom", organismes.ResponsableId);
            return View(organismes);
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
            ViewBag.ResponsableId = new SelectList(db.Responsable, "Id", "Nom", organismes.ResponsableId);
            return View(organismes);
        }

        // POST: Organismes/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "AccesLigue")]
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
            ViewBag.ResponsableId = new SelectList(db.Responsable, "Id", "Nom", organismes.ResponsableId);
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
        [Authorize(Roles = "AccesLigue")]
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
