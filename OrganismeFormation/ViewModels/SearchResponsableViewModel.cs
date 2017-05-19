using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class SearchResponsableViewModel
    {

        public SearchResponsableViewModel()
        {
            organisme = new Organismes();
        }
        
        public Organismes organisme { get; set; }
        public decimal OrganismeId { set; get; }

        [Required]
        [StringLength(16, MinimumLength = 16)]
        public String NumeroLicence { get; set; }

    }
}