using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Apogee.Models
{
    public class LoginViewComponent : ViewComponent 
    {
        private IDal dal;

        public IViewComponentResult Invoke()
        {
            /*Utilisateur utilisateur = dal.RecupererUtilisateur(9);*/
            Utilisateur utilisateur = new Utilisateur();
            {
                utilisateur.Nom = "Fekete";
                utilisateur.Prenom = "Floriant";
            }
            return View("_Utilisateur", utilisateur);
        }
    }
}
