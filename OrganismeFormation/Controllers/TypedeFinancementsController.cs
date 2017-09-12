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
    public class TypedeFinancementsController : Controller
    {
        private GestionOFEntities db = new GestionOFEntities();

        // GET: TypedeFinancements
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.TypedeFinancements.ToList());
        }

        // GET: TypedeFinancements/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFinancements typedeFinancements = db.TypedeFinancements.Find(id);
            if (typedeFinancements == null)
            {
                return HttpNotFound();
            }
            return View(typedeFinancements);
        }

        // GET: TypedeFinancements/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypedeFinancements/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Libelle")] TypedeFinancements typedeFinancements)
        {
            if (ModelState.IsValid)
            {
                db.TypedeFinancements.Add(typedeFinancements);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(typedeFinancements);
        }

        // GET: TypedeFinancements/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFinancements typedeFinancements = db.TypedeFinancements.Find(id);
            if (typedeFinancements == null)
            {
                return HttpNotFound();
            }
            return View(typedeFinancements);
        }

        // POST: TypedeFinancements/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Libelle")] TypedeFinancements typedeFinancements)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typedeFinancements).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(typedeFinancements);
        }

        // GET: TypedeFinancements/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFinancements typedeFinancements = db.TypedeFinancements.Find(id);
            if (typedeFinancements == null)
            {
                return HttpNotFound();
            }
            return View(typedeFinancements);
        }

        // POST: TypedeFinancements/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            TypedeFinancements typedeFinancements = db.TypedeFinancements.Find(id);
            db.TypedeFinancements.Remove(typedeFinancements);
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
