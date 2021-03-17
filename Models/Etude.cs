using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Apogee.Models
{
    [Table("Etude")]

    public class Etude
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("FK_id_collaborateur")]
        public int IdCollab { get; set; }

        [Column("intitule")]
        [MinLength(3)]
        [MaxLength(30)]
        [Required(ErrorMessage = "Veuillez entrer l'intitulé de la formation")]
        [Display(Name = "Intitulé")]
        public string Intitule { get; set; }

        [Column("etablissement")]
        [Required(ErrorMessage = "Veuillez entrer un établissement pour votre formation")]
        [MinLength(3)]
        [MaxLength(30)]
        [Display(Name = "Établissement")]
        public string Etablissement { get; set; }

        [Column("date_debut")]
        [Required(ErrorMessage = "Veuillez entrer une date de début")]
        [Display(Name = "Date de début")]
        public DateTime Date_debut { get; set; }

        [Column("date_fin")]
        [Required(ErrorMessage = "Veuillez entrer une date de début")]
        [Display(Name = "Date de fin")]
        public DateTime Date_fin { get; set; }

        [Column("ville")]
        public string Ville { get; set; }

        [Column("domaine")]
        public string Domaine { get; set; }

        [Column("fic_diplome")]
        [Display(Name = "Diplôme")]
        public string Diplome { get; set; }

        public string DateDebutMMAA
        {
            get
            {
                string dateDebutMMAA = Date_debut.ToString("y");
                return dateDebutMMAA;
            }
        }
        public string DateFinMMAA
        {
            get
            {
                string dateFinMMAA = Date_fin.ToString("y");
                return dateFinMMAA;
            }
        }

    }
}