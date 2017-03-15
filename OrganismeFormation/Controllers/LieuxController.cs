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
    public class LieuxController : Controller
    {
        private GestionOFEntities db = new GestionOFEntities();

        // GET: Lieux
        public ActionResult Index()
        {
            return View(db.Lieux.ToList());
        }

        // GET: Lieux/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lieux lieux = db.Lieux.Find(id);
            if (lieux == null)
            {
                return HttpNotFound();
            }
            return View(lieux);
        }

        // GET: Lieux/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lieux/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Adresse,CodePostal,Ville,Telephone,Email")] Lieux lieux)
        {
            if (ModelState.IsValid)
            {
                db.Lieux.Add(lieux);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lieux);
        }

        // GET: Lieux/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lieux lieux = db.Lieux.Find(id);
            if (lieux == null)
            {
                return HttpNotFound();
            }
            return View(lieux);
        }

        // POST: Lieux/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Adresse,CodePostal,Ville,Telephone,Email")] Lieux lieux)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lieux).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lieux);
        }

        // GET: Lieux/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lieux lieux = db.Lieux.Find(id);
            if (lieux == null)
            {
                return HttpNotFound();
            }
            return View(lieux);
        }

        // POST: Lieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Lieux lieux = db.Lieux.Find(id);
            db.Lieux.Remove(lieux);
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
