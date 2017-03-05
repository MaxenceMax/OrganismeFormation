using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrganismeFormation.Models
{
    public class AjoutResponsableModel
    {

        [Required]
        [Display(Name = "Identifiant")]
        public string Licence { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required]
        [Display(Name = "Prenom")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Mail")]
        public string AdresseEmail { get; set; }

        public decimal id { get; set; }

        [Required]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Required]
        [Display(Name = "Ligue")]
        public string Ligue { get; set; }




    }
}