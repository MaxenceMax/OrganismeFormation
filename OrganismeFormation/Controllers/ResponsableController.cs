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

            OrganismeModel organismeModel = new OrganismeModel();
            organismeModel.Responsable = resp;

            return View(organismeModel);
        }

    }


}