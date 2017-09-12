﻿using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OrganismeFormation.Controllers
{
    public class FormationsController : Controller
    {

        GestionOFEntities db = new GestionOFEntities();

        // GET: Formations
        [Authorize(Roles = "Responsable")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Responsable")]
        public ActionResult AddSimpleFormation(decimal id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypedeFormations type = db.TypedeFormations.Find(id);
            ViewBag.Libelle = type.Libelle;

            Formations formation = initializeFormation(id);

            return View(formation);
        }

        [HttpPost]
        [Authorize(Roles = "Responsable")]
        public ActionResult AddSimpleFormation(Formations formation)
        {
            if (ModelState.IsValidField("DateDebut") && ModelState.IsValidField("DateFin") && ModelState.IsValidField("DateLimiteInscription") && ModelState.IsValidField("DateTestSelection") &&
               ModelState.IsValidField("NombreTotalHeures") && ModelState.IsValidField("NbreHeureCentre") && ModelState.IsValidField("NbrHeureStructAccueil") &&
               ModelState.IsValidField("TotalHeureFOAD") && ModelState.IsValidField("HeureELearning") && ModelState.IsValidField("TotalAutresHeures") &&
               ModelState.IsValidField("TypeHeureContenu") && ModelState.IsValidField("CoutComplet") && ModelState.IsValidField("NbreHeureAutreEspace") &&
               ModelState.IsValidField("Personnel.Nom") && ModelState.IsValidField("Personnel.Prenom") && ModelState.IsValidField("Personnel.Email") && ModelState.IsValidField("Personnel.Telephone") &&
               ModelState.IsValidField("Lieux.Ville") && ModelState.IsValidField("Lieux.Adresse") && ModelState.IsValidField("Lieux.CodePostal")
               )
            {
                // Mise en place de l'organisateur pédagogique
                Personnel p = new Personnel();
                p.Nom = formation.Personnel.Nom;
                p.Prenom = formation.Personnel.Prenom;
                p.Email = formation.Personnel.Email;
                p.Telephone = formation.Personnel.Telephone;

                Lieux l = new Lieux();
                l.Adresse = formation.Lieux.Adresse;
                l.CodePostal = formation.Lieux.CodePostal;
                l.Ville = formation.Lieux.Ville;
                l.Telephone = formation.Lieux.Telephone;

                formation.LieuSiNonPorteuse = l.Id;
                formation.IdOrganisateurpeda = formation.Personnel.Id;
                formation.FormationEnded = false;

                db.Lieux.Add(l);
                db.Personnel.Add(p);
                db.Formations.Add(formation);
                db.SaveChanges();
                return backToGoodFormation((decimal)formation.OrganismeId, (decimal)formation.TypedeFormationsId);
            }
            TypedeFormations type = db.TypedeFormations.Find(formation.TypedeFormationsId);
            ViewBag.Libelle = type.Libelle;
            return View(formation);
        }

        private ActionResult backToGoodFormation(decimal org, decimal id)
        {
            if(id == 1)
            {
                return RedirectToAction("EtatFormationAC", "AccesResponsable", new { id = org });
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Responsable")]
        public ActionResult ShowASAC(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formations formation = db.Formations.Find(id);

            return View(formation);
        }



        private Formations initializeFormation(decimal id)
        {

            // recupération de l'organisme auquel sera rattaché notre nouvelle formation
            Organismes orga = TempData["model"] as Organismes;
            // récupération du responsable pour le préremplir comme responsable pédagogique
            Responsable rp = db.Responsable.Find(((Responsable)Session["Responsable"]).Id);

            Formations formation = new Formations();
            formation.TypedeFormationsId = id;
            formation.OrganismeId = orga.Id;

            Personnel pers = new Personnel();
            pers.Nom = rp.Nom;
            pers.Prenom = rp.Prenom;
            pers.Email = rp.Email;
            pers.Telephone = rp.Telephone;

            formation.Personnel = pers;

            return formation;
        }

    }
}