using OrganismeFormation.Models;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public class AccesLigueController : Controller
    {

        private GestionOFEntities db = new GestionOFEntities();

        // GET: AccesLigue
        [Authorize(Roles ="AccesLigue")]
        public ActionResult Home()
        {
            Ligues tmp = db.Ligues.Find(((Ligues)Session["Ligue"]).Id);
            ViewBag.libelle = tmp.Libelle;
            return View();
        }

        [Authorize(Roles = "AccesLigue")]
        public ActionResult ParametreLigue()
        {
            Ligues tmp = db.Ligues.Find(((Ligues)Session["Ligue"]).Id);
            return View(tmp);
        }

        // POST: Ligues/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "AccesLigue")]
        public ActionResult ParametreLigue(Ligues ligue)
        {
            if (ModelState.IsValid)
            {
                var tmp = db.Ligues.Find(ligue.Id);
                db.Ligues.Attach(tmp);
                if (tmp.login != ligue.login)
                {
                    if (db.Ligues.Any(a => a.login == ligue.login))
                    {
                        ModelState.AddModelError("login", "Un responsabe de ligue avec cet identifiant existe déjà, veuillez en saisir un nouveau.");
                        return View(ligue);
                    }
                    tmp.login = ligue.login;
                }
                if (tmp.password != ligue.password)
                {
                    tmp.password = encrypt(ligue.password);
                }
                tmp.Libelle = ligue.Libelle;
                tmp.email = ligue.email;
                db.SaveChanges();
                return RedirectToAction("Home");
            }
            return View(ligue);
        }


        private String encrypt(string mdp)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(mdp);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string hashedText = BitConverter.ToString(hashedBytes);
            return hashedText;

        }
    }
}