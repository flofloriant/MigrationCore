using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{
    [Table("Statut_Contrat")]
    public class Statut_Contrat
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("code_pays")]
        [Required(ErrorMessage = "Please enter your Code_pays")]
        [Display(Name = "Code pays :")]
        public string Code_pays { get; set; }
        [Column("nom")]
        [Required(ErrorMessage = "Please enter your type")]
        [Display(Name = "Nom :")]
        public string Nom { get; set; }
    }
}