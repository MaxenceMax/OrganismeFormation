using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OrganismeFormation.Models;
using System.Text;
using System.Security.Cryptography;

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
        public ActionResult Create([Bind(Include = "Id,Libelle,login,password,email")] Ligues ligues)
        {
            if (ModelState.IsValid)
            {
                if (db.Ligues.Any(a => a.login == ligues.login))
                {
                    ModelState.AddModelError("login", "Un responsabe de ligue avec cet identifiant existe déjà, veuillez en saisir un nouveau.");
                    return View(ligues);
                } 
                ligues.password = encrypt(ligues.password);
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
        public ActionResult Edit([Bind(Include = "Id,Libelle,login,password,email")] Ligues ligues)
        {
            if (ModelState.IsValid)
            {
                var tmp = db.Ligues.Find(ligues.Id);
                db.Ligues.Attach(tmp);
                tmp.email = ligues.email;
                tmp.login = tmp.login;
                tmp.Libelle = tmp.Libelle;
                if (tmp.password != ligues.password)
                    tmp.password = encrypt(ligues.password);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ligues);
        }

        private String encrypt(string mdp)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(mdp);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string hashedText = BitConverter.ToString(hashedBytes);
            return hashedText;

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
