using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Apogee.Models
{

    [Table("Collaborateur")]
    public class Collaborateur
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("matricule")]
        [Required(ErrorMessage = "Veuillez saisir votre numéro de matricule svp")]
        [Display(Name = "Matricule :")]
        public int Matricule { get; set; }

        [Column("civilite")]
        [Required(ErrorMessage = "Veuillez saisir votre civilité svp")]
        [Display(Name = "Civilité :")]
        public int Civilite { get; set; }

        [Column("nom")]
        [Required(ErrorMessage = "Veuillez saisir votre nom svp")]
        [MinLength(3)]
        [MaxLength(30)]
        [Display(Name = "Nom :")]
        public string Nom { get; set; }


        [Column("prenom")]
        [Required(ErrorMessage = "Veuillez saisir votre prénom svp")]
        [MinLength(3)]
        [MaxLength(30)]
        [Display(Name = "Prénom :")]
        public string Prenom { get; set; }

        [Column("fic_photo")]
        [Display(Name = "Photo :")]
        public string Fic_photo { get; set; }

        [Column("fic_autorisation_photo")]
        [Display(Name = "Autorisation photo :")]

        public string Fic_autorisation_photo { get; set; }

        [Column("FK_id_agence")]
        [Display(Name = "Agence rattachée :")]
        public int FK_id_agence { get; set; }


        [Column("adresse")]
        [Required(ErrorMessage = "Veuillez saisir votre adresse svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Adresse :")]
        public string Adresse { get; set; }

        [Column("code_postal")]
        [Required(ErrorMessage = "Veuillez saisir votre code postal svp")]
        [MinLength(5)]
        [MaxLength(10)]
        [Display(Name = "Code postal :")]
        public string Code_postal { get; set; }

        [Column("ville")]
        [Required(ErrorMessage = "Veuillez saisir votre ville svp")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Ville :")]
        public string Ville { get; set; }

        [Column("pays")]
        [Required(ErrorMessage = "Veuillez saisir votre pays svp")]
        [MinLength(3)]
        [MaxLength(30)]
        [Display(Name = "Pays :")]
        public string Pays { get; set; }

        [Column("date_naissance")]
        [Required(ErrorMessage = "Veuillez saisir votre date de naissance svp")]
        [DataType(DataType.Date, ErrorMessage = "Date obligatoire")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date_naissance
        {
            get; set;
        }


        [Column("email")]
        [Required(ErrorMessage = "Veuillez saisir votre émail personnel svp")]
        [MinLength(5)]
        [MaxLength(50)]
        [Display(Name = "Email personnel :")]
        [EmailAddress]
        public string Email
        {
            get; set;

        }

        [Column("email_travail")]
        [Required(ErrorMessage = "Veuillez saisir votre émail Apside svp")]
        [MinLength(5)]
        [MaxLength(50)]
        [EmailAddress]
        [Display(Name = "Email Apside :")]

        public string Email_travail
        {
            get; set;

        }

        [Column("tel")]
        [Required(ErrorMessage = "Veuillez saisir votre téléphone personnel svp")]
        [MinLength(10)]
        [MaxLength(20)]
        [Phone]
        [Display(Name = "Téléphone perso :")]
        public string Tel { get; set; }

        [Column("tel_travail")]
        [Required(ErrorMessage = "Veuillez saisir votre téléphone du travail svp")]
        [MinLength(10)]
        [MaxLength(20)]
        [Display(Name = "Téléphone travail :")]
        [Phone]
        public string Tel_travail { get; set; }

        [Column("FK_id_statut")]
        [Display(Name = "Statut :")]
        public int FK_id_statut { get; set; }

        [Column("FK_id_contrat")]
        [Display(Name = "Contrat :")]
        public int FK_id_contrat { get; set; }

        [Column("FK_id_niveau")]
        [Display(Name = "Niveau :")]
        public int FK_id_niveau { get; set; }

        [Column("FK_id_coefficient")]
        [Display(Name = "Coefficient :")]
        public int FK_id_coefficient { get; set; }

        [Column("salaire_brut_mens")]
        [Required(ErrorMessage = "Veuillez saisir votre salaire brut mensuel svp")]
        [Display(Name = "Salaire brut mensuel :")]
        public double Salaire_brut_mens { get; set; }

        [Column("date_fin_mission")]
        [Display(Name = "Date fin mission :")]
        public DateTime? Date_fin_mission { get; set; }

        [Column("date_embauche")]
        [Required(ErrorMessage = "Veuillez saisir votre date d'embauche svp")]       
        [DataType(DataType.Date, ErrorMessage = "Date obligatoire")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date embauche :")]
        public DateTime Date_embauche { get; set; }
        [Column("date_fin_contrat")]
        [Display(Name = "Date fin contrat :")]
        [DataType(DataType.Date, ErrorMessage = "Date obligatoire")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date_fin_contrat { get; set; }
        [Column("archive")]
        public bool Archive { get; set; }
        [Column("salaire_bru_ann")]
        [Required(ErrorMessage = "Veuillez saisir votre salaire brut annuel svp")]
        [Display(Name = "Salaire brut annuel :")]
        public double Salaire_bru_ann { get; set; }
        [Column("charge")]
        [Display(Name = "Disponibilité :")]
        public int Charge { get; set; }

        [Column("suffixe_homonyme")]
        public int Suffixe_homonyme { get; set; }

        [Column("abilite_defense")]
        [Display(Name = "Abilité défense :")]
        public bool Abilite_defense { get; set; }

        [Column("FK_id_commercial")]
        [Display(Name = "Commercial attachée :")]
        public int FK_id_commercial { get; set; }

        [Display(Name = "Nom Prénom :")]
        public string NomEntier
        {
            get
            {
                string nomEntier = Nom + " " + Prenom;
                return nomEntier;
            }
        }
        //  string nomEntier = Nom.Trim() + " " + Prenom.Trim();
        public int Statut
        {
            get
            {
                if (Date_fin_mission.HasValue)
                {
                    string test1 = DateTime.Now.ToString("yyyyMMdd");
                    string test2 = Date_fin_mission.Value.ToString("yyyyMMdd");
                    int result = string.Compare(test1, test2);
                    if (result > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        DateTime date2 = DateTime.Now.AddMonths(3);
                        test1 = date2.ToString("yyyyMMdd");
                        result = string.Compare(test1, test2);
                        if (result > 0)
                        {
                            return 2;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
