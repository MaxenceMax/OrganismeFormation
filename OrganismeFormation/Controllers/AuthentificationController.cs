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
            StatutConnection state = ValidateUser(model.Login, model.Password);
            if (!state.Connected)
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
                if(state.isLigue)
                    userClaims.AddRange(LoadRolesAccesLigue(model.Login));
                else
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
                if (state.isLigue)
                    return RedirectToAction("Home", "AccesLigue");
                else
                    return RedirectToAction("Home", "AccesResponsable");

            }
            
        }


        private IEnumerable<Claim> LoadRolesAdmin(string login)
        {
            yield return new Claim(ClaimTypes.Role, "Admin");

        }

        private IEnumerable<Claim> LoadRolesResponsable(string login)
        {
            GestionOFEntities db = new GestionOFEntities();
            var resp = db.Responsable.Where(a => a.Licence == login).First();
            System.Diagnostics.Debug.WriteLine("ID RESP" + resp.Nom);
            Session["Responsable"] = resp;
            yield return new Claim(ClaimTypes.Role, "Responsable");
        }

        private IEnumerable<Claim> LoadRolesAccesLigue(string login)
        {
            GestionOFEntities db = new GestionOFEntities();
            Session["Ligue"] = db.Ligues.Where(a => a.login == login);
            yield return new Claim(ClaimTypes.Role, "AccesLigue");

        }


        private StatutConnection ValidateUser(string login, string password)
        {

            StatutConnection state = new StatutConnection();
            state.Connected = false;
            state.isLigue = false;
            var ctx = new GestionOFEntities();
            var pass = encrypt(password);
            state.Connected =  ctx.Admin.Where(a => a.login == login && a.password == pass).Count() == 1;
            if(!state.Connected)
                state.Connected = ctx.Responsable.Where(a => a.Licence == login && a.Password == pass).Count() == 1;
            if(!state.Connected)
            {
                state.Connected = ctx.Ligues.Where(a => a.login == login && a.password == pass).Count() == 1;
                state.isLigue = true;
            }
            return state;
        }


        [HttpGet]
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            Session.Clear();

            // Rediriger vers la page d'accueil :
            return RedirectToAction("Index", "Home");
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