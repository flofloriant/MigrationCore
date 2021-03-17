using Apogee.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Apogee.ViewModels
{
    public class UserViewModel
    {
        public Utilisateur User { get; set; }

        [Required(ErrorMessage ="Veuillez saisir votre ancien mot de passe")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage ="Veuillez entrer un mot de passe")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="Les deux mots de passe sont differents, réessayez !")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}