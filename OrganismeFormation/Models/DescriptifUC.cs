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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class DescriptifUC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DescriptifUC()
        {
            this.ResultatUc = new HashSet<ResultatUc>();
        }
    
        public decimal Id { get; set; }

        [Required]
        public string Libelle { get; set; }

        public Nullable<decimal> Resultat { get; set; }

        [Required]
        [DisplayName("Note maximale")]
        [Range(0, int.MaxValue, ErrorMessage = "Veuillez renseigner une note maximale.")]
        public Nullable<decimal> ResultatMax { get; set; }

        public Nullable<decimal> FormationsId { get; set; }
        public virtual Formations Formations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ResultatUc> ResultatUc { get; set; }
    }
}
