using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OrganismeFormation.Models
{
    public class OrganismeModel
    {

        public Organismes Organismes { get; set; }

        public PresidentOrganisme PresidentOF { get; set; }

        public Personnel CoordinateurOF { get; set; }

        public Personnel DirecteurOF { get; set; }

        public Lieux LieuOF { get; set; }

        public Decimal OrganismeId { get; set; }


    }
}