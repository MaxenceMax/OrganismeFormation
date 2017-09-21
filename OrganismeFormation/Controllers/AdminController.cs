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
using System.Reflection;
using System.Collections;

namespace OrganismeFormation.Controllers
{
    public class AdminController : Controller
    {
        private const string CSV_SEPARATOR = ",";

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

        public ActionResult GetCsvLigue(decimal id)
        {
            var form = db.Ligues.Find(id);
            return File(Encoding.ASCII.GetBytes(CSVExport(form, null, new List<object>())), "text/csv", "Ligue" + id + ".csv");
        }

        public ActionResult GetCsvOrganisme(decimal id)
        {
            var form = db.Organismes.Find(id);
            return File(Encoding.ASCII.GetBytes(CSVExport(form, null, new List<object>())), "text/csv", "Organisme" + id + ".csv");
        }

        public ActionResult GetCsvFormation(decimal id)
        {
            var form = db.Formations.Find(id);
            return File(Encoding.ASCII.GetBytes(CSVExport(form, null, new List<object>())), "text/csv", "Formation" + id + ".csv");
        }
        
        private string CSVExport(object obj, Type parentType, List<object> alreadySerialized)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbEnTete = new StringBuilder();
            StringBuilder sbSousObjets = new StringBuilder();

            Type t = obj.GetType();
            PropertyInfo[] pi = t.GetProperties();

            alreadySerialized.Add(obj);

            for (int index = 0; index < pi.Length; index++)
            {
                if (pi[index].PropertyType.IsAssignableFrom(typeof(string)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(DateTime)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(int)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(decimal)) ||
                    pi[index].PropertyType.IsAssignableFrom(typeof(bool)))
                {

                    sbEnTete.Append(pi[index].Name);

                    if (!pi[index].PropertyType.IsAssignableFrom(typeof(bool)))
                        sb.Append(pi[index].GetValue(obj, null));
                    else
                    {
                        if (pi[index].GetValue(obj, null) != null && (bool)pi[index].GetValue(obj, null))
                        {
                            sb.Append("Oui");
                        }
                        else
                        {
                            sb.Append("Non");
                        }
                    }
                    if (index < pi.Length - 1)
                    {
                        sb.Append(CSV_SEPARATOR);
                        sbEnTete.Append(CSV_SEPARATOR);
                    }
                    else
                    {
                        sb.Append("\n");
                        sbEnTete.Append("\n");
                    }
                }
                else if (pi[index].PropertyType.Name.Contains("ICollection") && pi[index].GetValue(obj) != null)
                {
                    var objs = (pi[index].GetValue(obj) as IEnumerable);
                    foreach (object o in objs)
                    {
                        if (!alreadySerialized.Contains(o))
                            sbSousObjets.AppendLine(CSVExport(o, t, alreadySerialized));
                    }
                }
                else if (pi[index].GetValue(obj) != null && pi[index].GetValue(obj).GetType() != parentType && !alreadySerialized.Contains(pi[index].GetValue(obj)))
                {
                    sbSousObjets.Append(CSVExport(pi[index].GetValue(obj), t, alreadySerialized));
                }
            }

            return obj.GetType().Name.Split('_')[0] + "\r\n" + "\r\n" + sbEnTete.ToString() + "\n" + sb.ToString() + "\r\n" + "\r\n" + sbSousObjets.ToString();
        }
    }
}