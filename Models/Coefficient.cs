using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{
    [Table("Coefficient")]
    public class Coefficient
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("valeur")]
        [Required(ErrorMessage = "Please enter your valeur")]
        [Display(Name = "Valeur :")]
        public string Valeur { get; set; }

        [Column("FK_id_niveau")]
        public int FK_id_niveau { get; set; }

    }
}