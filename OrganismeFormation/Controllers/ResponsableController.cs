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
using System.Diagnostics;

namespace OrganismeFormation.Controllers
{
    public class ResponsableController : Controller
    {
        public ActionResult HomeResponsable()
        {
            return View();
        }



        [AllowAnonymous]
        public ActionResult Organisme()
        {

            OrganismeDataContext bd = new OrganismeDataContext();

            var claimIdentity = User.Identity as ClaimsIdentity;

            var nomResponsable = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var resp = bd.Responsable.First(a => a.Licence == nomResponsable);

            var org = bd.Organismes.First(a => a.ResponsableId == resp.Id);
            OrganismeModel organismeModel = new OrganismeModel();
            
            organismeModel.Organismes = org;
            organismeModel.OrganismeId = org.Id;

            return View(organismeModel);
        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult Organisme(OrganismeModel model)
        {

            OrganismeDataContext bd = new OrganismeDataContext();


            //On reprend l'objet Organismes et on met les infos pour l'update
            var claimIdentity = User.Identity as ClaimsIdentity;
            var nomResponsable = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var resp = bd.Responsable.First(a => a.Licence == nomResponsable);
            var org = bd.Organismes.First(a => a.ResponsableId == resp.Id);

            org.NumeroDeclaration = model.Organismes.NumeroDeclaration;
            org.AnneeDeclaration = model.Organismes.AnneeDeclaration;

            org.Lieux.Adresse = model.Organismes.Lieux.Adresse;
            org.Lieux.CodePostal = model.Organismes.Lieux.CodePostal;
         

            bd.SubmitChanges();


            return RedirectToAction("HomeResponsable", "Responsable");
        }

    }
}