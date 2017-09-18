using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class CandidatTuteurViewModel
    {
        [Required]
        [DisplayName("Numéro de licence du candidat")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Le numéro de licence n'est pas au bon format.")]
        public string LicenceCandidat { get; set; }

        [DisplayName("Numéro de licence du tuteur")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Le numéro de licence n'est pas au bon format.")]
        public string LicenceTuteur { get; set; }

        [Required]
        public decimal FormationId { get; set; }
    }
}