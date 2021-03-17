using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{
    [Table("Niveau")]
    public class Niveau
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("code_pays")]
        [Required(ErrorMessage = "Please enter your Code_pays")]
        [Display(Name = "Code pays :")]
        public string Code_pays { get; set; }
        [Column("valeur")]
        [Required(ErrorMessage = "Please enter your valeur")]
        [Display(Name = "Valeur :")]
        public string Valeur { get; set; }
    }
}