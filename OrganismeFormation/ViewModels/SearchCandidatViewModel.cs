using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class SearchCandidatViewModel
    {

        [Required]
        [StringLength(16, MinimumLength = 16)]
        public String NumeroLicence { get; set; }

        public Candidats candidat { get; set; }


    }
}