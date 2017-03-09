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



        //Route pour afficher les responsables
        [Authorize(Roles = "Admin")]
        public ActionResult ListeResponsable()
        {
            OrganismeDataContext bd = new OrganismeDataContext();
            var all = bd.Responsable;
            ResponsableViewModel vm = new ResponsableViewModel
            {
                ListeDesResponsables = all.ToList(),
            };
            return View(vm);
        }



        //Route pour ajouter un responsable
        [Authorize(Roles = "Admin")]
        public ActionResult AjoutResponsable()
        {
            return View();
        }

        [Authorize(Roles ="Admin")]
        public ActionResult ModificationResponsable(Decimal id)
        {
            OrganismeDataContext bd = new OrganismeDataContext();
            var temp = bd.Responsable.First(a => a.Id == id);
            AjoutResponsableModel resp = new AjoutResponsableModel();
            resp.Licence = temp.Licence;
            resp.Nom = temp.Nom;
            resp.Prenom = temp.Prenom;
            resp.Telephone = temp.Telephone;
            resp.AdresseEmail = temp.Email;
            resp.Ligue = temp.Organismes.First().Ligue;
            resp.id = temp.Id;

            return View(resp);
        }

        //Modification Responsable
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ModificationResponsable(AjoutResponsableModel model)
        {

            OrganismeDataContext bd = new OrganismeDataContext();
            var resp = bd.Responsable.First(a => a.Id == model.id);
            resp.Licence = model.Licence;
            resp.Nom = model.Nom;
            resp.Prenom = model.Prenom;
            resp.Telephone = model.Telephone;
            resp.Email = model.AdresseEmail;
            resp.Organismes.First().Ligue = model.Ligue;

            
            bd.SubmitChanges();

            return RedirectToAction("ListeResponsable", "Admin");
        }



        //Ajout Responsable
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AjoutResponsable(Models.AjoutResponsableModel model)
        {

            OrganismeDataContext bd = new OrganismeDataContext();

            Responsable resp = new Responsable();
            Organismes org = new Organismes();

            // Mise à jour des données d'un responsable
            resp.Nom = model.Nom;
            resp.Prenom = model.Prenom;
            resp.Licence = model.Licence;
            resp.Password = encrypt(model.Password);
            resp.Email = model.AdresseEmail;
            resp.Telephone = model.Telephone;


            org.Ligue = model.Ligue;
            org.Responsable = resp;
            bd.Organismes.InsertOnSubmit(org);
            bd.Responsable.InsertOnSubmit(resp);
            bd.SubmitChanges();

            return RedirectToAction("ListeResponsable", "Admin");

        }


        [Authorize(Roles = "Admin")]
        public ActionResult SuppressionResponsable(decimal id)
        {
            OrganismeDataContext bd = new OrganismeDataContext();
            var resp = bd.Responsable.First(a => a.Id == id);
            if (resp.Organismes.First().Lieux == null)
            {
                bd.Organismes.DeleteOnSubmit(resp.Organismes.First());
            }
            bd.Responsable.DeleteOnSubmit(resp);
            bd.SubmitChanges();
            return RedirectToAction("ListeResponsable", "Admin");

        }


        public String encrypt(string mdp)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(mdp);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            string hashedText = BitConverter.ToString(hashedBytes);
            return hashedText;

        }


    }
}