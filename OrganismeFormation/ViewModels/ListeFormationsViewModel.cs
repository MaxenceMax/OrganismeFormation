using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class ListeFormationsViewModel
    {

        public ListeFormationsViewModel()
        {
            AC = new Collection<Formations>();
            AS = new Collection<Formations>();
            CFEB = new Collection<Formations>();
            CQP = new Collection<Formations>();
            BPJEPS = new Collection<Formations>();
            DEJEPS = new Collection<Formations>();
            DESJEPS = new Collection<Formations>();
        }

        public Organismes organisme { get; set; }

        public IEnumerable<Formations> AC { get; set; }
        public IEnumerable<Formations> AS { get; set; }
        public IEnumerable<Formations> CFEB { get; set; }
        public IEnumerable<Formations> CQP { get; set; }
        public IEnumerable<Formations> BPJEPS { get; set; }
        public IEnumerable<Formations> DEJEPS { get; set; }
        public IEnumerable<Formations> DESJEPS { get; set; }
    }
}