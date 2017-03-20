using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class AddResponsableViewModel
    {

        public AddResponsableViewModel()
        {
            organisme = new Organismes();
            responsable = new Responsable();
        }


        public Organismes organisme { get; set; }

        public Responsable responsable { get; set; }

        public decimal organismeId { get; set; }

    }
}