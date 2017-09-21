using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class ExportViewModel
    {
        public ExportViewModel()
        {
            ligues = new List<Ligues>();
            organismes = new List<Organismes>();
            formations = new List<Formations>();
        }

        public int LigueId { get; set; }
        public List<Ligues> ligues { get; set; }

        public int OrganismesId { get; set; }
        public List<Organismes> organismes { get; set; }

        public int FormationsId { get; set; }
        public List<Formations> formations { get; set; }
    }
}