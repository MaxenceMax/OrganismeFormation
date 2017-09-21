using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using OrganismeFormation.Models;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using OrganismeFormation.ViewModels;


namespace OrganismeFormation.Controllers
{
    public class AdminController : Controller
    {

        private GestionOFEntities db = new GestionOFEntities();
        // GET: Admin
        [Authorize (Roles = "Admin")]
        public ActionResult HomeAdmin()
        {
            return View();
        }

        private String encrypt(string mdp)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(mdp);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string hashedText = BitConverter.ToString(hashedBytes);
            return hashedText;

        }

        [Authorize(Roles = "Admin")]
        public ActionResult ParametreAdmin()
        {
            Admin tmp = db.Admin.Find(((Admin)Session["Admin"]).id);
            return View(tmp);
        }

        // POST: Ligues/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ParametreAdmin(Admin admin)
        {
            
            if (ModelState.IsValid)
            {
                var tmp = db.Admin.Find(admin.id);
                db.Admin.Attach(tmp);
                if (tmp.password != admin.password)
                {
                    tmp.password = encrypt(admin.password);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("HomeAdmin", "Admin");
        }


        [Authorize(Roles ="Admin")]
        public ActionResult Export()
        {
            ExportViewModel m = new ExportViewModel();
            m.ligues =db.Ligues.ToList();
            return View(m);
        }

        [Authorize(Roles="Admin")]
        [HttpPost]
        public ActionResult Export(ExportViewModel export)
        {
            TempData["ViewModel"] = export;
            return RedirectToAction("Export2");
        }

        [Authorize(Roles="Admin")]
        public ActionResult Export2()
        {
            ExportViewModel model = TempData["ViewModel"] as ExportViewModel;
            if(model == null)
            {
                return RedirectToAction("Export");
            }
            TempData["ViewModel"] = model;
            model.ligues = db.Ligues.ToList();
            model.organismes = new List<Organismes>(db.Ligues.Find((decimal)model.LigueId).Organismes);
            return View(model);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult Export2(ExportViewModel model)
        {
            TempData["ViewModel"] = model;
            return RedirectToAction("Export3");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Export3()
        {
            ExportViewModel model = TempData["ViewModel"] as ExportViewModel;
            if (model == null)
            {
                return RedirectToAction("Export");
            }
            TempData["ViewModel"] = model;
            model.ligues = db.Ligues.ToList();
            model.organismes = db.Organismes.ToList();
            model.formations = new List<Formations>(db.Organismes.Find((decimal)model.OrganismesId).Formations);
            return View(model);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult SearchCandidat()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult SearchCandidat(SearchCandidatViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.CandidatsFormations.Where(c => c.NumeroLicence == model.NumeroLicence).Count() > 0)
                {
                    return RedirectToAction("Index", "CandidatsFormations", new { id = db.CandidatsFormations.Where(c => c.NumeroLicence == model.NumeroLicence).FirstOrDefault().Id });
                }
                else
                {
                    ModelState.AddModelError("", "Candidat Inconnu");
                }
            }
            return View(model);
        }
    }
}