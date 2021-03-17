using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Apogee.Models
{
    public class Experiences
    {
        public Experiences()
        {
        }

        public Experiences(int id, int fK_id_collaborateur, string nom_client, string description, DateTime date_debut, DateTime date_fin, string intitule, string ville, string pays, int charge, string logoClient, string domaine)
        {
            Id = id;
            FK_id_collaborateur = fK_id_collaborateur;
            Nom_client = nom_client;
            Description = description;
            Date_debut = date_debut;
            Date_fin = date_fin;
            Intitule = intitule;
            Ville = ville;
            Pays = pays;
            Charge = charge;
            LogoClient = logoClient;
            Domaine = domaine;
        }

        [Required]
        public int Id { get; set; }
        public int FK_id_collaborateur { get; set; }
        [Required(ErrorMessage = "Veuillez saisir le nom du client svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Nom du client :")]
        public string Nom_client { get; set; }
        [Display(Name = "Description :")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Veuillez saisir la date début svp")]
        [DataType(DataType.Date, ErrorMessage = "Date obligatoire")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date_debut
        {
            get; set;
        }
        [Required(ErrorMessage = "Veuillez saisir la date fin svp")]
        [DataType(DataType.Date, ErrorMessage = "Date obligatoire")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date_fin
        {
            get; set;
        }
        [Required(ErrorMessage = "Veuillez saisir l'intitulé svp")]
        [MinLength(3)]
        [MaxLength(100)]
        [Display(Name = "Intitulé :")]
        public string Intitule { get; set; }

        [Required(ErrorMessage = "Veuillez saisir votre ville svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Ville :")]
        public string Ville { get; set; }
        [Required(ErrorMessage = "Veuillez saisir votre pays svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Pays :")]
        public string Pays { get; set; }
        [Display(Name = "Disponibilité :")]
        public int Charge { get; set; }

        [Display(Name = "Logo :")]
        public string LogoClient { get; set; }

        [Display(Name = "Télécharger le logo du client")]
        public IFormFile Logo { get; set; }       
       
        
        [Display(Name = "Domaine :")]
        public string Domaine { get; set; }
        
    }
}