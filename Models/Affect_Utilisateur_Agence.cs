using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{
    [Table("Affect_Utilisateur_Agence")]
    public class Affect_Utilisateur_Agence
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("fk_id_utilisateur")]

        public int fk_id_utilisateur { get; set; }

        [Column("fk_id_agence")]

        public int fk_id_agence { get; set; }
    }
}