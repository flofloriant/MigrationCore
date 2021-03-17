using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Resources;
using System.Web;

namespace Apogee.Models
{
    [Table("Utilisateur")]
    public class Utilisateur
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("prenom")]
        public string Prenom { get; set; }

        [Column("email")]
        [Required(ErrorMessage = "Please enter your email")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [Column("mot_de_passe")]
        [Required(ErrorMessage = "Please enter your password")]
        [Display(Name = "Password :")]
        [DataType(DataType.Password)]
        public string MotDePasse { get; set; }

        [Column("profil")]
        public string Profil { get; set; }

        [Column("compteur_mdp_faux")]
        public int CompteurMDPFaux { get; set; }

        [Column("validite_mdp")]
        public DateTime? ValiditeMDP { get; set; }

        [Column("date_bloquage")]
        public DateTime? DateBloquage { get; set; }
        public Utilisateur(int id, string nom, string prenom)
        {
            Id = id;
            Nom = nom;
            Prenom = prenom;
        }

        public Utilisateur()
        {
        }

        public string NomEntier
        {
            get
            {
                string nomEntier = Nom + " " + Prenom;
                return nomEntier;
            }
        }
    }
}