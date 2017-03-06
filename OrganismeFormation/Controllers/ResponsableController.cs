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


        //Route pour ajouter un responsable
        [Authorize(Roles = "Responsable")]
        public ActionResult Organisme()
        {

            return View();
        }

    }


}