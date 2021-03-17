using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apogee.Models
{
    [Table("Competence")]
    public class Competence
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("FK_id_collaborateur")]
        public int IdCollab { get; set; }

        [Column("nom")]
        [MaxLength(30)]
        public string Nom { get; set; }
        //j'ai du l'enlever pour que ça passe sinon pouvait même pas utiliser la bdd
        /*public IEnumerable<SelectListItem> Noms { get; set; }*/

        [Column("type")]
        [MaxLength(1)]
        public string Type { get; set; }

        [Column("niveau")]
        public int Niveau { get; set; }

        [Column("fic_certificat")]
        [MaxLength(512)]
        public string Certificat { get; set; }

        [ForeignKey("IdCollab")]
        public virtual Collaborateur Collaborateur { get; set; }

        internal string NiveauStrLang()
        {
            string lvlStr = string.Empty;
            switch (Niveau)
            {
                case 2:
                    lvlStr = Apogee.Resources.Resources.LanguageLevel2;
                    break;
                case 3:
                    lvlStr = Apogee.Resources.Resources.LanguageLevel3;
                    break;
                case 4:
                    lvlStr = Apogee.Resources.Resources.LanguageLevel4;
                    break;
                case 5:
                    lvlStr = Apogee.Resources.Resources.LanguageLevel5;
                    break;
                default:
                    lvlStr = Apogee.Resources.Resources.LanguageLevel1;
                    break;
            }
            return lvlStr;
        }
    }
}