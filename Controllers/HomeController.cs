using Apogee.Data;
using Apogee.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Apogee.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }
        

        /// <summary>
        /// Affichage de la vue d'accueil
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AccessDb()
        {
            Collaborateur collab = new Collaborateur()
            {
                Abilite_defense = true,
                Matricule = 0,
                Civilite = 1,
                Nom = "Coucou",
                Prenom = "Tranquille",
                Adresse = "10 rue Neuve",
                Code_postal = "68100",
                Ville = "Mulhouse",
                Pays = "France",
                Email = "flo@sfr.fr",
                Email_travail = "flo1@sfr.fr",
                Tel = "0658925539",
                Tel_travail = "0758925539",
                Date_naissance = DateTime.Now,
                Date_embauche = DateTime.Now,
                Salaire_brut_mens = 1000,
                Salaire_bru_ann = 12000,
                Suffixe_homonyme = 2,
                Charge = 100
            };
            _db.Collaborateurs.Add(collab);
            _db.SaveChanges();
            Collaborateur collabfound = _db.Collaborateurs.FirstOrDefault(m => m.Nom == "coucou");
            return View(collabfound);
        }
        public ActionResult NewCollab()

        {

            return RedirectToAction("NewCollab", "Collaborator");
        }
    }
}