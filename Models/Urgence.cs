using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{
    [Table("Urgence")]
    public class Urgence
    {
        public Urgence()
        {
        }

        public Urgence(int id, int fK_id_collaborateur, string nom, string prenom, string adresse, string tel_fixe, string tel_port, string tel_travail, string lien)
        {
            Id = id;
            FK_id_collaborateur = fK_id_collaborateur;
            Nom = nom;
            Prenom = prenom;
            Adresse = adresse;
            Tel_fixe = tel_fixe;
            Tel_port = tel_port;
            Tel_travail = tel_travail;
            Lien = lien;
        }

        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("FK_id_collaborateur")]       
        public int FK_id_collaborateur { get; set; }
        [Column("nom")]
        [Required(ErrorMessage = "Veuillez saisir votre nom svp")]
        [MinLength(3)]
        [MaxLength(30)]
        [Display(Name = "Nom :")]
        public string Nom { get; set; }
        [Column("prenom")]
        [Required(ErrorMessage = "Veuillez saisir votre prénom svp")]
        [MinLength(3)]
        [MaxLength(30)]
        [Display(Name = "Prenom :")]
        public string Prenom { get; set; }
        
        [Column("adresse")]
        [Required(ErrorMessage = "Veuillez saisir votre adresse svp")]
        [MinLength(3)]
        [MaxLength(150)]
        [Display(Name = "Adresse :")]
        public string Adresse { get; set; }     
        [Column("tel_fixe")]
        [Required(ErrorMessage = "Veuillez saisir votre téléphone fixe svp")]
        [MinLength(10)]
        [MaxLength(20)]
        [Phone]
        [Display(Name = "Telephone fixe :")]
        public string Tel_fixe { get; set; }
        [Column("tel_port")]
        [Required(ErrorMessage = "Veuillez saisir votre téléphone portable svp")]
        [MinLength(10)]
        [MaxLength(20)]
        [Phone]
        [Display(Name = "Telephone portable :")]
        public string Tel_port { get; set; }
        [Column("tel_travail")]
        
        [MinLength(10)]
        [MaxLength(20)]
        [Display(Name = "Telephone travail :")]
        [Phone]
        public string Tel_travail { get; set; }
        [Column("lien")]
        [Required(ErrorMessage = "Veuillez saisir votre lien svp")]
        [MinLength(3)]
        [MaxLength(20)]
        [Display(Name = "Lien :")]   
        public string Lien { get; set; }
       
    }
}