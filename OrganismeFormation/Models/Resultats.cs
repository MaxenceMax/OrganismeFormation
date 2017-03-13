//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrganismeFormation.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Resultats
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Resultats()
        {
            this.ResultatsUC = new HashSet<ResultatsUC>();
        }
    
        public decimal Id { get; set; }
        public decimal CandidatFormationId { get; set; }
        public Nullable<bool> FormationValidee { get; set; }
        public string NumeroDiplome { get; set; }
        public Nullable<bool> EPEF { get; set; }
        public Nullable<bool> EPMSP { get; set; }
        public decimal PassagesId { get; set; }
        public string FormationTerminee { get; set; }
        public string Abandon { get; set; }
    
        public virtual CandidatsFormations CandidatsFormations { get; set; }
        public virtual Passages Passages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultatsUC> ResultatsUC { get; set; }
    }
}
