using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{


    [Table("Poste")]
    public class Poste
    {
        public Poste()
        {
        }

        public Poste(int id, int fK_id_collaborateur, string nom_client, string description, DateTime date_debut, DateTime date_fin, string intitule, string ville, string pays, int charge, string logo, string domaine)
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
            Logo = logo;
            Domaine = domaine;
        }

        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column("FK_id_collaborateur")]
        public int FK_id_collaborateur { get; set; }

        [Column("nom_client")]
        [Required(ErrorMessage = "Veuillez saisir le nom du client svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Nom du client :")]
        public string Nom_client { get; set; }

        [Column("description")]
        [Display(Name = "Description :")]
        public string Description { get; set; }

        [Column("date_debut")]
        [Required(ErrorMessage = "Veuillez saisir la date début svp")]
        [DataType(DataType.Date, ErrorMessage = "Date obligatoire")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date_debut
        {
            get; set;
        }

        [Column("date_fin")]
        [Required(ErrorMessage = "Veuillez saisir la date fin svp")]
        [DataType(DataType.Date, ErrorMessage = "Date obligatoire")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date_fin
        {
            get; set;
        }

        [Column("intitule")]
        [Required(ErrorMessage = "Veuillez saisir l'intitulé svp")]
        [MinLength(3)]
        [MaxLength(100)]
        [Display(Name = "Intitulé :")]
        public string Intitule { get; set; }
        [Column("ville")]
        [Required(ErrorMessage = "Veuillez saisir votre ville svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Ville :")]
        public string Ville { get; set; }

        [Column("pays")]
        [Required(ErrorMessage = "Veuillez saisir votre pays svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Pays :")]
        public string Pays { get; set; }

        [Column("charge")]
        [Display(Name = "Disponibilité :")]
        public int Charge { get; set; }
        [Column("logo")]
        [Display(Name = "Logo :")]
        public string Logo { get; set; }

        [Column("domaine")]
        [Required(ErrorMessage = "Veuillez saisir le domaine svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Domaine :")]
        public string Domaine { get; set; }

       
    }
}
