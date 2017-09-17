using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class CandidatFormationViewModel
    {
        public CandidatFormationViewModel()
        {
            Tuteur = new Tuteurs();
            Candidat = new CandidatsFormations();
            Formation = new Formations();
            Sexe = new Sexes();

        }
        public Tuteurs Tuteur { get; set; }
        public CandidatsFormations Candidat { get; set; }
        public Formations Formation { get; set; }
        public Sexes Sexe { get; set; }
    }
}