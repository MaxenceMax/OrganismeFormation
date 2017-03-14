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


        ////Route pour afficher les responsables
        //[Authorize(Roles = "Admin")]
        //public ActionResult ListeResponsable()
        //{
        //    GestionOFEntities bd = new GestionOFEntities();
        //    var all = bd.Responsable;
        //    ResponsableViewModel vm = new ResponsableViewModel
        //    {
        //        ListeDesResponsables = all.ToList(),
        //    };
        //    return View(vm);
        //}



        ////Route pour ajouter un responsable
        //[Authorize(Roles = "Admin")]
        //public ActionResult AjoutResponsable()
        //{
        //    return View();
        //}

        //[Authorize(Roles ="Admin")]
        //public ActionResult ModificationResponsable(Decimal id)
        //{
        //    GestionOFEntities bd = new GestionOFEntities();
        //    var temp = bd.Responsable.First(a => a.Id == id);
        //    AjoutResponsableModel resp = new AjoutResponsableModel();
        //    resp.Licence = temp.Licence;
        //    resp.Nom = temp.Nom;
        //    resp.Prenom = temp.Prenom;
        //    resp.Telephone = temp.Telephone;
        //    resp.AdresseEmail = temp.Email;
        //    resp.id = temp.Id;

        //    return View(resp);
        //}

        ////Modification Responsable
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public ActionResult ModificationResponsable(AjoutResponsableModel model)
        //{

        //    GestionOFEntities bd = new GestionOFEntities();
        //    var resp = bd.Responsable.Find(model.id);
        //    bd.Responsable.Attach(resp);

        //    resp.Licence = model.Licence;
        //    resp.Nom = model.Nom;
        //    resp.Prenom = model.Prenom;
        //    resp.Telephone = model.Telephone;
        //    resp.Email = model.AdresseEmail;


        //    bd.SaveChanges();

        //    return RedirectToAction("ListeResponsable", "Admin");
        //}



        ////Ajout Responsable
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public ActionResult AjoutResponsable(Models.AjoutResponsableModel model)
        //{

        //    GestionOFEntities bd = new GestionOFEntities();

        //    Responsable resp = new Responsable();
        //    Organismes org = new Organismes();

        //    bd.Responsable.Attach(resp);
        //    bd.Organismes.Attach(org);


        //    // Mise à jour des données d'un responsable
        //    resp.Nom = model.Nom;
        //    resp.Prenom = model.Prenom;
        //    resp.Licence = model.Licence;
        //    resp.Password = encrypt(model.Password);
        //    resp.Email = model.AdresseEmail;
        //    resp.Telephone = model.Telephone;


        //    org.Responsable = resp;
        //    bd.SaveChanges();

        //    return RedirectToAction("ListeResponsable", "Admin");

        //}


        //[Authorize(Roles = "Admin")]
        //public ActionResult SuppressionResponsable(decimal id)
        //{
        //    GestionOFEntities bd = new GestionOFEntities();
        //    var resp = bd.Responsable.Find(id);
        //    bd.Responsable.Attach(resp);
        //    if (resp.Organismes.First().Lieux == null)
        //    {
        //        bd.Responsable.Remove(resp);
        //    }
        //    bd.SaveChanges();
        //    return RedirectToAction("ListeResponsable", "Admin");

        //}
    }
}