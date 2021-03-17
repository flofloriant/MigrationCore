using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{
    [Table("Formation")]
    public class Formation
    {
        [Column("num_demande")]
        [Key]
        public int Num_demande { get; set; }
        [Column("FK_id_collaborateur")]
        public int FK_id_collaborateur { get; set; }

        [Column("intitule")]
        public string Intitule { get; set; }
        [Column("certification")]
        public bool Certification { get; set; }
        [Column("date_demande")]
        public DateTime Date_demande { get; set; }
        [Column("date_acceptation")]
        public DateTime Date_acceptation { get; set; }
        [Column("FK_id_statut")]
        public int FK_id_statut { get; set; }
        [Column("date_debut")]
        public DateTime Date_debut { get; set; }
        [Column("date_fin")]
        public DateTime Date_fin { get; set; }
        [Column("validation")]
        public bool Validation { get; set; }
        [Column("organisme")]
        public string Organisme { get; set; }
        [Column("fic_certification")]
        public string Fic_certification { get; set; }
        [Column("fic_devis")]
        public string Fic_devis { get; set; }

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