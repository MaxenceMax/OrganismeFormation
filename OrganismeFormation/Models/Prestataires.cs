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
    
    public partial class Prestataires
    {
        public decimal Id { get; set; }
        public string Nom { get; set; }
        public Nullable<bool> MiseADispoInstallation { get; set; }
        public Nullable<bool> GestionPaieFormateurs { get; set; }
        public Nullable<bool> CoPortage { get; set; }
        public Nullable<bool> GestionAdministrative { get; set; }
        public string Autre { get; set; }
        public Nullable<decimal> LieuxId { get; set; }
    }
}
