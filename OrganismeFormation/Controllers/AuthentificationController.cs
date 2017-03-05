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
    public class AuthentificationController : Controller
    {
        // GET: Authentification
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Models.LoginViewModel model, string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //On regarde dans la fonction Validate pour donner la connexion à statut ainsi que admin ou ligue
            StatutConnection statut = ValidateUser(model.Login, model.Password);
            if (!statut.Connected)
            {
                ModelState.AddModelError(string.Empty, "Le nom d'utilisateur ou le mot de passe est incorrect.");
                return View(model);
            }


            // L'authentification est réussie, 
            // injecter les informations utilisateur dans le cookie d'authentification :
            var userClaims = new List<Claim>();
            // Identifiant utilisateur :
            userClaims.Add(new Claim(ClaimTypes.NameIdentifier, model.Login));
            // Rôles utilisateur :
            if (model.Login == "admin")
            {
                userClaims.AddRange(LoadRolesAdmin(model.Login));
            }
            else
            {
                userClaims.AddRange(LoadRolesResponsable(model.Login));
            }
            var claimsIdentity = new ClaimsIdentity(userClaims, DefaultAuthenticationTypes.ApplicationCookie);
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(claimsIdentity);

            if (model.Login == "admin")
            {
                return RedirectToAction("HomeAdmin", "Admin");
            }
            else {
                return RedirectToAction("HomeResponsable", "Responsable");
            }
            
        }


        private IEnumerable<Claim> LoadRolesAdmin(string login)
        {

            yield return new Claim(ClaimTypes.Role, "Admin");

        }

        private IEnumerable<Claim> LoadRolesResponsable(string login)
        {

            yield return new Claim(ClaimTypes.Role, "Responsable");

        }


        private StatutConnection ValidateUser(string login, string password)
        {

            OrganismeDataContext bd = new OrganismeDataContext();
            StatutConnection statut = new StatutConnection();
            statut.Connected = false;
            if (login == "admin")
            {
                statut.Connected = (from i in bd.Admin
                                    where i.login == login && i.password == password
                                    select i).Count() != 0;
            }
            else
            {
                statut.Connected = (from i in bd.Responsable
                                    where i.Licence == login && i.Password == password
                                    select i).Count() != 0;
            }
            return statut;
        }


        [HttpGet]
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();

            // Rediriger vers la page d'accueil :
            return RedirectToAction("Index", "Home");
        }



    }
}