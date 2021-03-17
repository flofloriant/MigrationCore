using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Apogee.Models
{
    public class Fonctions
    {
        [Required]
        [Display(Name = "Compétence fonctionnelle")]
        public string FuncSkill { get; set; }

        [Display(Name = "Niveau fonctionnel")]
        public int FuncNiveau { get; set; }
        public int FK_id_collaborateur { get; set; }
    }
}