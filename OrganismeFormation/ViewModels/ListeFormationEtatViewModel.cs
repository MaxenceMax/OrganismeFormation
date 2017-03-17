using OrganismeFormation.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace OrganismeFormation.ViewModels
{
    public class ListeFormationEtatViewModel
    {

        public ListeFormationEtatViewModel()
        {
            enCours = new Collection<Formations>();
            termine = new Collection<Formations>();
        }

        public IEnumerable<Formations> enCours { get; set; }
        public IEnumerable<Formations> termine { get; set; }
    }
}