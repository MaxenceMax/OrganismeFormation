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
    public class TypedeFormationsController : Controller
    {
        private GestionOFEntities db = new GestionOFEntities();

        [Authorize(Roles ="Admin")]
        // GET: TypedeFormations
        public ActionResult Index()
        {
            return View(db.TypedeFormations.ToList());
        }

        [Authorize(Roles = "Admin")]
        // GET: TypedeFormations/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFormations typedeFormations = db.TypedeFormations.Find(id);
            if (typedeFormations == null)
            {
                return HttpNotFound();
            }
            return View(typedeFormations);
        }

        [Authorize(Roles = "Admin")]
        // GET: TypedeFormations/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: TypedeFormations/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Libelle")] TypedeFormations typedeFormations)
        {
            if (ModelState.IsValid)
            {
                db.TypedeFormations.Add(typedeFormations);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typedeFormations);
        }

        // GET: TypedeFormations/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFormations typedeFormations = db.TypedeFormations.Find(id);
            if (typedeFormations == null)
            {
                return HttpNotFound();
            }
            return View(typedeFormations);
        }

        // POST: TypedeFormations/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Libelle")] TypedeFormations typedeFormations)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typedeFormations).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(typedeFormations);
        }

        // GET: TypedeFormations/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFormations typedeFormations = db.TypedeFormations.Find(id);
            if (typedeFormations == null)
            {
                return HttpNotFound();
            }
            return View(typedeFormations);
        }

        // POST: TypedeFormations/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            TypedeFormations typedeFormations = db.TypedeFormations.Find(id);
            db.TypedeFormations.Remove(typedeFormations);
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
