using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Apogee.Models
{
    public class Formations
    {
        [Required]
        public string Intitule { get; set; }
        [Required]
        public string Ville { get; set; }
        [Required]
        public string Domaine { get; set; }
        [Required]
        public string Etablissement { get; set; }
        public IFormFile UploadFile { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date de début")]
        [Required]
        public DateTime DateDeb { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Date de fin")]
        [Required]
        public DateTime DateFin { get; set; }
        public int FK_id_collaborateur { get; set; }
    }
}