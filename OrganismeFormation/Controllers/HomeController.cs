using OrganismeFormation.Models;
using OrganismeFormation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OrganismeFormation.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult NewPassword()
        {

            return View();
        }

        private static Random random = new Random();
        GestionOFEntities ctx = new GestionOFEntities();

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NewPassword(PasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (ctx.Ligues.Where(a => a.email == model.Email).Count() == 1)
                {
                    ModelState.AddModelError("", "Votre nouveau mot de passe a été envoyé sur votre adresse email.");
                    Ligues l = ctx.Ligues.Where(a => a.email == model.Email).FirstOrDefault();
                    ctx.Ligues.Attach(l);
                    String tmp = generator();
                    l.password = encrypt(tmp);
                    ctx.SaveChanges();
                    envoyer(l.email, l.Libelle, "", tmp);
                }
                else if (ctx.Responsable.Where(a => a.Email == model.Email).Count() == 1)
                {
                    ModelState.AddModelError("", "Votre nouveau mot de passe a été envoyé sur votre adresse email.");
                    Responsable r = ctx.Responsable.Where(a => a.Email == model.Email).FirstOrDefault();
                    ctx.Responsable.Attach(r);
                    String tmp = generator();
                    r.Password = encrypt(tmp);
                    ctx.SaveChanges();
                    envoyer(r.Email, r.Nom, r.Prenom, tmp);
                }
                else
                {
                    ModelState.AddModelError("", "Cette adresse email est introuvable.");
                }
            }
            return View(model);
        }

        private String envoyer(String adresse,String nom, String prenom, String password)
        {
            String retour = "";
            String signature = "<br><br>Fédération Française de Judo, Jujitsu, Kendo et Disciplines Associées<br>";
            signature = signature + "Association Loi 1901<br>";
            signature = signature + "21-25, avenue de la Porte de Châtillon - 75014 Paris<br>";
            signature = signature + "Tél : 01 40 52 16 16<br>";
            String message = "";


            MailMessage email = new MailMessage();
            email.From = new MailAddress("no-reply@licences-ffjudo.com");

            // Enregistrement des destinataires
            email.To.Add(new MailAddress(adresse));

            // structure du message
            String _message = "<div style=' font-family: Calibri;'>Bonjour " + prenom + " " + nom +
                ",<br><br>" +
                "votre demande de réinitialisation de mot de passe a été prise en compte. <br><br>" +
                "Voici votre nouveau mot de passe :<br>"+
                "<b style = 'font-size:19' >" + password + "</b ><br>" +
                "<br><br>Cordialement</div>";
            email.Subject = "Réinitialisation mot de passe Gestion des OF";
            message = "<div style=' font-family: Calibri;'>" + _message + signature + "</div>";
            email.Body = message;
            email.IsBodyHtml = true;
            //email.Priority = MailPriority.High;
            //SmtpClient client = new SmtpClient("195.154.95.219", 587);
            // Command line argument must the the SMTP host.
            SmtpClient client = new SmtpClient();
            //client.Port = 587;
            client.Host = "mail.sevenstorm.com";
            //client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("informations@licences-ffjudo.com", "nBD)dB3");
            try
            {
                client.Send(email);
            }
            catch (Exception ex)
            {
                retour = ex.Message;
            }
            return retour;
        }

        private String generator()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var tmp = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            return tmp;
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