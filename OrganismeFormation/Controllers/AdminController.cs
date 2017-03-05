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




        //Ajout Responsable
        [HttpPost]
        [AllowAnonymous]
        public ActionResult AjoutResponsable(Models.AjoutResponsableModel model)
        {

            OrganismeDataContext bd = new OrganismeDataContext();

            Models.Responsable resp = new Models.Responsable();

            resp.Nom = model.Nom;
            resp.Prenom = model.Prenom;
            resp.Licence = model.Licence;
            resp.Password = encrypt(model.Password);
            resp.Email = model.AdresseEmail;
            resp.Telephone = model.Telephone;


            bd.Responsable.InsertOnSubmit(resp);
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