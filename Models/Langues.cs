using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Apogee.Models
{
    public class Langues
    {
        [Required]
        public string Langue { get; set; }

        [Required]
        [Range(1, 5)]
        public int Niveau { get; set; }

        [Display(Name = "Télécharger un fichier")]
        public IFormFile UploadFile { get; set; }
        public int FK_id_collaborateur { get; set; }
    }
}