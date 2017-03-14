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
    public class LiguesController : Controller
    {
        private GestionOFEntities db = new GestionOFEntities();

        // GET: Ligues
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Ligues.ToList());
        }

        // GET: Ligues/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ligues ligues = db.Ligues.Find(id);
            if (ligues == null)
            {
                return HttpNotFound();
            }
            return View(ligues);
        }

        // GET: Ligues/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ligues/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Libelle,CodeLigue")] Ligues ligues)
        {
            if (ModelState.IsValid)
            {
                db.Ligues.Add(ligues);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ligues);
        }

        // GET: Ligues/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ligues ligues = db.Ligues.Find(id);
            if (ligues == null)
            {
                return HttpNotFound();
            }
            return View(ligues);
        }

        // POST: Ligues/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Libelle,CodeLigue")] Ligues ligues)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ligues).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ligues);
        }

        // GET: Ligues/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ligues ligues = db.Ligues.Find(id);
            if (ligues == null)
            {
                return HttpNotFound();
            }
            return View(ligues);
        }

        // POST: Ligues/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Ligues ligues = db.Ligues.Find(id);
            db.Ligues.Remove(ligues);
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
