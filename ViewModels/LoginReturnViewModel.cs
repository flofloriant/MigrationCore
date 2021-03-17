using Apogee.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Apogee.ViewModels
{
    public class LoginReturnViewModel
    {
        public Utilisateur Utilisateur { get; set; }
        public int NbErrorsLeft { get; set; }
        public double TimeBlocked { get; set; }
        public bool PasswordInValidity{ get; set; }
    }
}