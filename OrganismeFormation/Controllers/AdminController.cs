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


        //Ajout Responsable
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AjoutResponsable(Models.AjoutResponsableModel model)
        {

            OrganismeDataContext bd = new OrganismeDataContext();

            Models.Responsable resp = new Models.Responsable();
            Models.Organismes org = new Models.Organismes();

            resp.Nom = model.Nom;
            resp.Prenom = model.Prenom;
            resp.Licence = model.Licence;
            resp.Password = encrypt(model.Password);
            resp.Email = model.AdresseEmail;
            resp.Telephone = model.Telephone;

            bd.Responsable.InsertOnSubmit(resp);
            bd.SubmitChanges();

            org.Ligue = model.Ligue;
            var req = from i in bd.Responsable
                      orderby i.Id ascending
                      select i.Id;

            foreach (var detail in req)
            {
                org.ResponsableId = detail;
            }

            try
            {
                bd.Organismes.InsertOnSubmit(org);
                bd.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Provide for exceptions.
            }



            return RedirectToAction("ListeResponsable", "Admin");

        }


        [AllowAnonymous]
        public ActionResult SuppressionResponsable(decimal id)
        {


            OrganismeDataContext bd = new OrganismeDataContext();

            var req = from i in bd.Responsable
                      where i.Id == id
                      select i;


            foreach (var detail in req)
            {
                bd.Responsable.DeleteOnSubmit(detail);
            }

            try
            {
                bd.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Provide for exceptions.
            }

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