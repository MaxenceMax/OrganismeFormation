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
    public partial class Organismes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Organismes()
        {
            this.Formations = new HashSet<Formations>();
            this.Responsable = new HashSet<Responsable>();
        }
    
        public decimal Id { get; set; }
        [Required]
        [DisplayName("Libellé")]
        [StringLength(200, ErrorMessage = "Le libellé doit contenir entre 5 et 200 caractères.", MinimumLength = 5)]
        public string Libelle { get; set; }
        [Required]
        [DisplayName("Numéro de déclaration")]
        public string NumeroDeclaration { get; set; }
        [Required]
        [DisplayName("Année de déclaration")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "L'année n'est pas au bon format.")]
        public string AnneeDeclaration { get; set; }
        public Nullable<decimal> LieuxId { get; set; }
        public Nullable<decimal> PresidentId { get; set; }
        public Nullable<decimal> CoordinateurId { get; set; }
        public Nullable<decimal> DirecteurId { get; set; }
        public Nullable<decimal> LigueId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Formations> Formations { get; set; }
        public virtual Lieux Lieux { get; set; }
        public virtual Ligues Ligues { get; set; }
        public virtual Personnel Personnel { get; set; }
        public virtual Personnel Personnel1 { get; set; }
        public virtual PresidentOrganisme PresidentOrganisme { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Responsable> Responsable { get; set; }
    }
}
