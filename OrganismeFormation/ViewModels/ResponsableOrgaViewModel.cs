using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class ResponsableOrgaViewModel
    {

        public ResponsableOrgaViewModel()
        {
            responsable = new Models.Responsable();
        }
        public decimal OrganismeId { get; set; }
        public Models.Responsable responsable { get; set; }
        
    }
}