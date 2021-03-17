using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Apogee.Models
{
    public class Photo
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Veuillez choisir votre autorisation svp")]
        [Display(Name ="Télécharger l'autorisation")]
        public IFormFile Fic_autorisation_photo { get; set; }
        [Required(ErrorMessage = "Veuillez choisir votre photo svp")]
        [Display(Name = "Télécharger la photo")]
        public IFormFile Fic_photo { get; set; }

        public string Fic_photo_path { get; set; }
        public string Erreur { get; set; }
        public string Fic_autorisation_photoCollab { get; set; }
        public string Fic_photoCollab { get; set; }


    }
}