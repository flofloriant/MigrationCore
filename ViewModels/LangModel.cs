using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Apogee.ViewModels
{
    public class LangModel
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