using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace Apogee.Models
{
    public class Techniques
    {
        [Required]
        [Display(Name = "Compétence technique")]
        public string TechSkill { get; set; }

        [Required]
        [Display(Name = "Niveau technique")]
        public int TechNiveau { get; set; }
        public int FK_id_collaborateur { get; set; }
    }
}