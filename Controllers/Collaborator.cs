
using Apogee.Data;
using Apogee.Models;
using Apogee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Apogee.SaveAs
{
    public static class FileSaveExtension
    {
        public static void SaveAs(this IFormFile formFile, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                formFile.CopyTo(stream);
            }
        }
    }
}

namespace Apogee.Controllers
{
    using Apogee.SaveAs;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.ViewEngines;

    [Authorize]
    public class CollaboratorController : Controller
    {
        private readonly IDal dal;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        private ICompositeViewEngine _viewEngine;

        public CollaboratorController(ApplicationDbContext db, ICompositeViewEngine viewEngine, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _viewEngine = viewEngine;
        }


        #region NewCollaborateur en mode Get
        /// <summary>
        /// Création d'un collaboratuer par default

        /// </summary>
        /// <returns></returns>
        /// [ValidateAntiForgeryToken]
        /// 

        [HttpGet]
        public ActionResult NewCollab()
        {
            Collaborateur collaborateur = new Collaborateur();

            int idUtil = int.Parse(HttpContext.User.Identity.Name);
            collaborateur.Email = "email@apside.fr";
            collaborateur.Email_travail = "travail@email.fr";
            collaborateur.Code_postal = "0000000";
            collaborateur.Nom = "nom";
            collaborateur.Prenom = "prenom";
            collaborateur.Pays = "pays";
            collaborateur.Tel = "000000000000";
            collaborateur.Tel_travail = "000000000000";
            collaborateur.Ville = "ville";
            collaborateur.Matricule = 0000;
            collaborateur.Adresse = "adresse";
            collaborateur.Salaire_brut_mens = 0000;
            collaborateur.Salaire_bru_ann = 0000;
            collaborateur.Date_naissance = DateTime.Now;
            collaborateur.Date_embauche = DateTime.Now;
            collaborateur.Charge = 100;
            collaborateur.Civilite = 1;
            collaborateur.FK_id_agence = dal.ObtenirFirstAgence(idUtil);
            string pays = dal.PaysAgence(collaborateur.FK_id_agence);
            collaborateur.FK_id_contrat = dal.ObtenirFirstContrat(pays);
            collaborateur.FK_id_statut = dal.ObtenirFirstStatut(pays);
            collaborateur.FK_id_niveau = dal.ObtenirFirstNiveau(pays);
            collaborateur.FK_id_coefficient = dal.ObtenirFirstCoefficient(collaborateur.FK_id_niveau);
            collaborateur.Matricule = 0000;
            collaborateur.Abilite_defense = false;
            collaborateur.FK_id_commercial = dal.ObtientFirstCommercial(idUtil, "CO");
            int suffixe = 0;
            if (dal.NomPrenomExiste(collaborateur.Nom, collaborateur.Prenom))
            {
                suffixe = dal.NomPrenomMax(collaborateur.Nom, collaborateur.Prenom);
                suffixe++;
            }
            if (suffixe != 0)
            {
                collaborateur.Suffixe_homonyme = suffixe;
            }
            if (dal.CreerCollaborateur(collaborateur))
            {
                collaborateur.Id = dal.DernierIdCollab(collaborateur);
                int id = collaborateur.Id;
                collaborateur = TrimCollaborateur(collaborateur);
                ChargerTousLesModals(collaborateur);
                return View(collaborateur);
            }
            else
            {
                string messageTitre = Resources.Resources.SeriousError;
                string messageErreur = Resources.Resources.Reason + Resources.Resources.Reason1;
                string commentaireErreur = Resources.Resources.NotifyAdmin;
                ErreurGrave nouveauErreur = new ErreurGrave(messageTitre, messageErreur, commentaireErreur);
                return RedirectToAction("ErreurGrave", "Collaborator", new { nouveauErreur });
            }
        }
        #endregion
        #region EditCollaborateur en mode Get
        /// <summary>
        /// Affichage de la vue de nouveau collaborateur
        /// récuperation des tables correspondants Agence, Affect_Utilisateurs_Agence,
        /// Coefficient, Contrat, Niveau, Utilisateur et Statut_Contrat
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult EditCollab(int? id)
        {

            if (id.HasValue)
            {
                Collaborateur collaborateur = dal.RecupererCollaborateur(id.Value);
                if (collaborateur != null)
                {
                    collaborateur = TrimCollaborateur(collaborateur);
                    ChargerTousLesModals(collaborateur);
                    return View(collaborateur);
                }
                else
                {
                    string messageTitre = Resources.Resources.SeriousError;
                    string messageErreur = Resources.Resources.Reason + Resources.Resources.Reason2;
                    string commentaireErreur = Resources.Resources.NotifyAdmin;
                    ErreurGrave nouveauErreur = new ErreurGrave(messageTitre, messageErreur, commentaireErreur);
                    return RedirectToAction("ErreurGrave", "Collaborator", new { nouveauErreur });
                }
            }
            else
            {
                string messageTitre = Resources.Resources.SeriousError;
                string messageErreur = Resources.Resources.Reason + Resources.Resources.Reason2;
                string commentaireErreur = Resources.Resources.NotifyAdmin;
                ErreurGrave nouveauErreur = new ErreurGrave(messageTitre, messageErreur, commentaireErreur);
                return RedirectToAction("ErreurGrave", "Collaborator", new { nouveauErreur });

            }
        }
        #endregion
        #region ModifCollab est utiliser pour NewCollab et EditCollab en mode Get
        public ActionResult ModifCollab(Collaborateur model)
        {
            return PartialView(model);
        }
        #endregion
        #region New Collaborateur en mode Post       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collaborateur"></param>
        /// <returns></returns>
        /// 
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult NewCollab(Collaborateur model)
        {
            ChargerTousLesModals(model);
            // contrôle email Apside
            if (!TestMail(model.Email))
            {
                ViewData["Message"] = Resources.Resources.EmailInvalide;
                return View(model);
            }
            // contrôle email travail
            if (!TestMail(model.Email_travail))
            {
                ViewData["Message"] = Resources.Resources.EmailWorkInvalide;
                return View(model);
            }
            // contrôle saisie generale des champs obligatoire
            if (!TestChampCollaborateur(model) ||
                      (!ModelState.IsValid))
            {
                ViewData["Message"] = Resources.Resources.MandatoryInput;
                return View(model);
            }
            // contrôle matricule
            string matricule = Convert.ToString(model.Matricule);

            if (matricule.Length != 4 || string.IsNullOrWhiteSpace(matricule) || !Int32.TryParse(matricule, out int _))
            {
                ViewData["Message"] = Resources.Resources.RegistrationNumberInvalid;
                return View(model);
            }
            if (dal.MatriculeExiste(model.Matricule, model.Id))
            {
                ViewData["Message"] = Resources.Resources.MatriculeInvalid;
                return View(model);
            }
            // contrôle salaire brut mensuel
            string salaireBrutMens = Convert.ToString(model.Salaire_brut_mens);

            if (salaireBrutMens.Length > 11 || string.IsNullOrWhiteSpace(salaireBrutMens) || !float.TryParse(salaireBrutMens, out float _))
            {
                ViewData["Message"] = Resources.Resources.MonthlyPayInvalid;
                return View(model);
            }
            // contrôle salaire brut annuel
            string salaireBrutAnn = Convert.ToString(model.Salaire_bru_ann);

            if (salaireBrutAnn.Length > 13 || string.IsNullOrWhiteSpace(salaireBrutAnn) || !float.TryParse(salaireBrutAnn, out float _))
            {
                ViewData["Message"] = Resources.Resources.MonthlyPayInvalid;
                return View(model);
            }
            // contrôle date de naissance   
            if (!TestDate(model.Date_naissance))
            {
                ViewData["Message"] = Resources.Resources.PlsDate_naissance;
                return View(model);
            }
            // contrôle date d'embauche     
            if (!TestDate(model.Date_embauche))
            {
                ViewData["Message"] = Resources.Resources.PlsDate_embauche;
                return View(model);
            }
            // contrôle cohérence entre saisie agence et commerciale
            if (!dal.CommercialExiste(model.FK_id_agence, model.FK_id_commercial))
            {

                ViewData["Message"] = Resources.Resources.BusinessInvalid;
                return View(model);
            }
            // récuperation du pays de l'agence selectionner
            Agence agence = dal.ObtenirAgence(model.FK_id_agence);
            // contrôle cohérence entre saisie agence et contrat           
            if (!dal.ContratExiste(model.FK_id_contrat, agence.Code_pays))
            {

                ViewData["Message"] = Resources.Resources.ContractInvalid;
                return View(model);
            }
            // contrôle cohérence entre saisie agence et statut           
            if (!dal.StatutExiste(model.FK_id_statut, agence.Code_pays))
            {

                ViewData["Message"] = Resources.Resources.StatutInvalid;
                return View(model);
            }
            // contrôle cohérence entre saisie agence et niveau           
            if (!dal.NiveauExiste(model.FK_id_niveau, agence.Code_pays))
            {

                ViewData["Message"] = Resources.Resources.GradeInvalid;
                return View(model);
            }
            // contrôle cohérence entre saisie niveau et coefficient           
            if (!dal.CoefficientExiste(model.FK_id_coefficient, model.FK_id_niveau))
            {

                ViewData["Message"] = Resources.Resources.CoefficientsInvalid;
                return View(model);
            }


            int suffixe = 0;
            if (dal.NomPrenomExiste(model.Nom, model.Prenom))
            {
                suffixe = dal.NomPrenomMax(model.Nom, model.Prenom);
                suffixe++;
            }
            if (suffixe != 0)
            {
                model.Suffixe_homonyme = suffixe;
            }
            model.Charge = 100;

            if (dal.MiseAJourCollab(model))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string messageTitre = Resources.Resources.SeriousError;
                string messageErreur = Resources.Resources.Reason + Resources.Resources.Reason1;
                string commentaireErreur = Resources.Resources.NotifyAdmin;
                ErreurGrave nouveauErreur = new ErreurGrave(messageTitre, messageErreur, commentaireErreur);
                return RedirectToAction("ErreurGrave", "Collaborator", new { nouveauErreur });
            }

        }

        #endregion
        #region EditCollaborateur en mode Post
        /// <summary>
        /// EditCollab Edition Collaborateur en mode Post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult EditCollab(Collaborateur model)
        {
            ChargerTousLesModals(model);


            // contrôle email Apside
            if (!TestMail(model.Email))
            {
                ViewData["Message"] = Resources.Resources.EmailInvalide;
                return View(model);

            }
            // contrôle email travail
            if (!TestMail(model.Email_travail))
            {
                ViewData["Message"] = Resources.Resources.EmailWorkInvalide;
                return View("ModifCollab", model);

            }
            // contrôle saisie generale des champs obligatoire
            if (!TestChampCollaborateur(model) ||
                (!ModelState.IsValid))
            {
                ViewData["Message"] = Resources.Resources.MandatoryInput;
                return View(model);
            }
            // contrôle matricule
            string matricule = Convert.ToString(model.Matricule);

            if (matricule.Length != 4 || string.IsNullOrWhiteSpace(matricule) || !Int32.TryParse(matricule, out int _))
            {
                ViewData["Message"] = Resources.Resources.RegistrationNumberInvalid;
                return View("EditCollab", model);
            }
            if (dal.MatriculeExiste(model.Matricule, model.Id))
            {
                ViewData["Message"] = Resources.Resources.MatriculeInvalid;
                return View("EditCollab", model);
            }
            // contrôle salaire brut mensuel
            string salaireBrutMens = Convert.ToString(model.Salaire_brut_mens);

            if (salaireBrutMens.Length > 11 || string.IsNullOrWhiteSpace(salaireBrutMens) || !float.TryParse(salaireBrutMens, out float _))
            {
                ViewData["Message"] = Resources.Resources.MonthlyPayInvalid;
                return View(model);
            }
            // contrôle salaire brut annuel
            string salaireBrutAnn = Convert.ToString(model.Salaire_bru_ann);

            if (salaireBrutAnn.Length > 13 || string.IsNullOrWhiteSpace(salaireBrutAnn) || !float.TryParse(salaireBrutAnn, out float _))
            {
                ViewData["Message"] = Resources.Resources.MonthlyPayInvalid;
                return View(model);
            }
            // contrôle date de naissance   
            if (!TestDate(model.Date_naissance))
            {
                ViewData["Message"] = Resources.Resources.PlsDate_naissance;
                return View(model);
            }
            // contrôle date d'embauche     
            if (!TestDate(model.Date_embauche))
            {
                ViewData["Message"] = Resources.Resources.PlsDate_embauche;
                return View(model);
            }
            // contrôle cohérence entre saisie agence et commerciale
            if (!dal.CommercialExiste(model.FK_id_agence, model.FK_id_commercial))
            {

                ViewData["Message"] = Resources.Resources.BusinessInvalid;
                return View(model);
            }
            // récuperation du pays de l'agence selectionner
            Agence agence = dal.ObtenirAgence(model.FK_id_agence);
            // contrôle cohérence entre saisie agence et contrat           
            if (!dal.ContratExiste(model.FK_id_contrat, agence.Code_pays))
            {

                ViewData["Message"] = Resources.Resources.ContractInvalid;
                return View(model);
            }
            // contrôle cohérence entre saisie agence et statut           
            if (!dal.StatutExiste(model.FK_id_statut, agence.Code_pays))
            {

                ViewData["Message"] = Resources.Resources.StatutInvalid;
                return View(model);
            }
            // contrôle cohérence entre saisie agence et niveau           
            if (!dal.NiveauExiste(model.FK_id_niveau, agence.Code_pays))
            {

                ViewData["Message"] = Resources.Resources.GradeInvalid;
                return View(model);
            }
            // contrôle cohérence entre saisie niveau et coefficient           
            if (!dal.CoefficientExiste(model.FK_id_coefficient, model.FK_id_niveau))
            {

                ViewData["Message"] = Resources.Resources.CoefficientsInvalid;
                return View(model);
            }
            int suffixe = 0;
            if (dal.NomPrenomExiste(model.Nom, model.Prenom))
            {
                suffixe = dal.NomPrenomMax(model.Nom, model.Prenom);
                suffixe++;
            }
            if (suffixe != 0)
            {
                model.Suffixe_homonyme = suffixe;
            }
            model.Charge = 100;

            if (dal.MiseAJourCollab(model))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string messageTitre = Resources.Resources.SeriousError;
                string messageErreur = Resources.Resources.Reason + Resources.Resources.Reason1;
                string commentaireErreur = Resources.Resources.NotifyAdmin;
                ErreurGrave nouveauErreur = new ErreurGrave(messageTitre, messageErreur, commentaireErreur);
                return RedirectToAction("ErreurGrave", "Collaborator", new { nouveauErreur });
            }
        }
        #endregion        
        #region Tous les vues partielle de ModifCollab
        /// <summary>
        /// Vue partial pour table agences par utilisateur
        /// </summary>
        /// <returns></returns>
        public ActionResult AgencesVue(Collaborateur model)
        {
            // récuperation tous les agences du profil d'utilisateur
            int idUtil = int.Parse(HttpContext.User.Identity.Name);
            List<Agence> listeDesAgences = dal.ObtientTousLesAgences(idUtil);
            int count = dal.ObtenirPaysAgence(listeDesAgences);
            ViewBag.listeDesAgences = new SelectList(listeDesAgences, "Id", "Nom", selectedValue: model.FK_id_agence);
            if (count > 1)
            {
                ViewData["paysChange"] = 1;

            }
            else
            {
                ViewData["paysChange"] = 0;
            }
            return PartialView(model);
        }
        /// <summary>
        /// Vue partial pour table commerciales par agences 
        /// </summary>
        /// <returns></returns>
        public ActionResult CommercialsVue(Collaborateur model)
        {
            // récuperation tous les agences du profil d'utiisateur       
            List<Utilisateur> listeDesCommercials = dal.ObtientTousLesCommercials(model.FK_id_agence, "CO");
            ViewBag.listeDesCommercials = new SelectList(listeDesCommercials, "Id", "NomEntier", selectedValue: model.FK_id_commercial);
            return PartialView("CommercialsVue", model);
        }
        /// <summary>
        /// Vue partial pour table contrats par pays 
        /// </summary>
        /// <returns></returns>
        public ActionResult ContratsVue(Collaborateur model)
        {
            // récuperation tous les Contrats par premier pays
            string pays = dal.PaysAgence(model.FK_id_agence);
            List<Contrat> resultatsContrat = dal.ObtientTousLesContrats(pays);
            ViewBag.listeDesContrats = new SelectList(resultatsContrat, "Id", "Nom", selectedValue: model.FK_id_contrat);
            return PartialView(model);
        }
        /// <summary>
        /// Vue partial pour table contrats par pays 
        /// </summary>
        /// <returns></returns>
        public ActionResult StatutsVue(Collaborateur model)
        {
            // récuperation tous les Statuts par premier pays
            string pays = dal.PaysAgence(model.FK_id_agence);
            List<Statut_Contrat> listeDesStatuts = dal.ObtientTousLesStatutContrat(pays);
            ViewBag.listeDesStatuts = new SelectList(listeDesStatuts, "Id", "Nom", selectedValue: model.FK_id_statut);
            return PartialView(model);
        }
        /// <summary>
        /// Vue partial pour table niveaux par pays 
        /// </summary>
        /// <returns></returns>
        public ActionResult NiveauxVue(Collaborateur model)
        {
            // récuperation tous les Niveaux par premier pays
            string pays = dal.PaysAgence(model.FK_id_agence);
            List<Niveau> listeDesNiveaux = dal.ObtientTousLesNiveaux(pays);
            ViewBag.listeDesNiveaux = new SelectList(listeDesNiveaux, "Id", "Valeur", selectedValue: model.FK_id_niveau);
            return PartialView(model);
        }
        /// <summary>
        /// Vue partial pour table coefficient par niveaux par pays 
        /// </summary>
        /// <returns></returns>
        public ActionResult CoefficientsVue(Collaborateur model)
        {
            // récuperation tous les agences du profil d'utiisateur       
            List<Coefficient> resultatsCoefficients = dal.ObtientTousLesCoefficients(model.FK_id_niveau);
            ViewBag.listeDesCoefficients = new SelectList(resultatsCoefficients, "Id", "Valeur", selectedValue: model.FK_id_coefficient);
            return PartialView(model);
        }
        /// <summary>
        /// Vue partial pour la date de naissance 
        /// </summary>
        /// <returns></returns>
        public ActionResult DateNaissanceVue(Collaborateur model)
        {
            ViewBag.dateNaissance = model.Date_naissance.ToString("yyyy-MM-dd ");
            return PartialView(model);
        }
        /// <summary>
        /// Vue partial pour la date d'embauche 
        /// </summary>
        /// <returns></returns>
        public ActionResult DateEmbaucheVue(Collaborateur model)
        {
            ViewBag.dateEmbauche = model.Date_embauche.ToString("yyyy-MM-dd ");
            return PartialView(model);
        }
        /// <summary>
        /// On met la liste des commercials à jour si agence a changé
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCommercial(Collaborateur model)
        {
            List<Utilisateur> listeDesCommercials = dal.ObtientTousLesCommercials(model.FK_id_agence, "CO");
            return Json(new { listeDesCommercials }/*, JsonRequestBehavior.AllowGet*/);
        }
        /// <summary>
        /// On met la liste des coefficient à jour si niveau a changé
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetCoefficient(Collaborateur model)
        {
            List<Coefficient> resultatsCoefficients = dal.ObtientTousLesCoefficients(model.FK_id_niveau);
            return Json(new { resultatsCoefficients }/*, JsonRequestBehavior.AllowGet*/);
        }
        /// <summary>
        /// On met la liste des contrats à jour si agence a changé et il y a plusieurs pays
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetContrat(Collaborateur model)
        {
            string pays = dal.PaysAgence(model.FK_id_agence);
            List<Contrat> resultatsContrat = dal.ObtientTousLesContrats(pays);
            return Json(new { resultatsContrat } /*, JsonRequestBehavior.AllowGet*/);
        }
        /// <summary>
        /// On met la liste des statuts à jour si agence a changé et il y a plusieurs pays
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetStatut(Collaborateur model)
        {
            string pays = dal.PaysAgence(model.FK_id_agence);
            List<Statut_Contrat> listeDesStatuts = dal.ObtientTousLesStatutContrat(pays);
            return Json(new { listeDesStatuts }/*, JsonRequestBehavior.AllowGet*/);
        }
        /// <summary>
        /// On met la liste des niveaux à jour si agence a changé et il y a plusieurs pays
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetNiveau(Collaborateur model)
        {
            string pays = dal.PaysAgence(model.FK_id_agence);
            List<Niveau> listeDesNiveaux = dal.ObtientTousLesNiveaux(pays);
            return Json(new { listeDesNiveaux }/*, JsonRequestBehavior.AllowGet*/);
        }
        #endregion
        #region Controle du format mail
        public static bool TestMail(string nom)
        {
            int compteurMail = 0;
            int compteurPoint = 0;
            foreach (char c in nom)
            {
                if (c == '@')
                { compteurMail++; }
                if (c == '.' && compteurMail > 0)
                { compteurPoint++; }
            }
            if (string.IsNullOrWhiteSpace(nom) || nom.Length > 80 || nom.Length < 8 || compteurMail != 1 || compteurPoint < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
        #region Controle du validite des champs d'entree du collaborateur
        public static bool TestChampCollaborateur(Collaborateur model)
        {
            if (string.IsNullOrWhiteSpace(model.Adresse) ||
               string.IsNullOrWhiteSpace(model.Code_postal) ||
               string.IsNullOrWhiteSpace(model.Nom) ||
               string.IsNullOrWhiteSpace(model.Prenom) ||
               string.IsNullOrWhiteSpace(model.Pays) ||
               string.IsNullOrWhiteSpace(model.Tel) ||
               string.IsNullOrWhiteSpace(model.Tel_travail) ||
               model.Salaire_brut_mens == 0 ||
               model.Salaire_bru_ann == 0 ||
               model.Adresse.Length < 3 ||
               model.Adresse.Length > 50 ||
               model.Nom.Length < 3 ||
               model.Nom.Length > 30 ||
               model.Prenom.Length < 3 ||
               model.Prenom.Length > 30 ||
               model.Adresse.Length < 3 ||
               model.Adresse.Length > 50 ||
               model.Code_postal.Length < 5 ||
               model.Code_postal.Length > 10 ||
               model.Ville.Length < 3 ||
               model.Ville.Length > 50 ||
               model.Pays.Length < 3 ||
               model.Pays.Length > 30 ||
               model.Tel.Length < 10 ||
               model.Tel.Length > 20 ||
               model.Tel_travail.Length < 10 ||
               model.Tel_travail.Length > 20 ||
               string.IsNullOrWhiteSpace(model.Ville))
            {

                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
        #region Importer Exporter une photo
        #region PhotoVue Importer en mode Get
        /// <summary>
        /// Vue photo pour importer ou exporter photo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult PhotoView(Collaborateur model)
        {
            Photo photo = new Photo();
            photo.Erreur = string.Empty;
            photo.Fic_photo_path = model.Fic_photo;
            photo.Id = model.Id;
            photo.Fic_autorisation_photoCollab = model.Fic_autorisation_photo;
            photo.Fic_photoCollab = model.Fic_photo;
            return PartialView("PhotoVue", photo);

        }

        #endregion
        #region PhotoVue Suppression en mode Get
        /// <summary>
        /// Vue photo pour importer ou exporter photo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult PhotoViewDelete(Collaborateur model)
        {
            Photo photo = new Photo();
            photo.Erreur = string.Empty;
            photo.Fic_photo_path = model.Fic_photo;
            photo.Id = model.Id;
            photo.Fic_autorisation_photoCollab = model.Fic_autorisation_photo;
            photo.Fic_photoCollab = model.Fic_photo;
            return PartialView("PhotoVueDelete", photo);

        }
        #endregion
        #region PhotoVue importer en mode Post
        /// <summary>
        /// Importer la photo        
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult PhotoVue(Photo model)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            Collaborateur collaborateurPhoto = new Collaborateur();
            collaborateurPhoto.Id = model.Id;
            string path1 = string.Empty;
            string path2 = string.Empty;
            if (dal.PhotoExiste(model.Id))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.AlreadyUpload;
            }
            else
            // contrôle saisie generale des champs obligatoire
            {
                if (model.Fic_autorisation_photo == null ||
                  model.Fic_autorisation_photo.Length < 1 ||
                  model.Fic_photo == null ||
                  model.Fic_photo.Length < 1)
                {
                    ajaxListViewModel.errorMsg = Resources.Resources.FileUpload;

                }
                else
                {
                    try
                    {
                        string contentRootPath = _webHostEnvironment.ContentRootPath;
                        string path = Path.Combine(contentRootPath, "Images\\docs");
                        //Vérifie si un fichier Images\docs existe, si non le créer
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        path1 = Path.Combine(path, Path.GetFileName(model.Id + model.Fic_autorisation_photo.FileName));
                        model.Fic_autorisation_photo.SaveAs(path1);  
                        path1 = "/Images/docs/" + Path.GetFileName(model.Id + model.Fic_autorisation_photo.FileName);
                        path2 = Path.Combine(path, Path.GetFileName(model.Id + model.Fic_photo.FileName));
                        model.Fic_photo.SaveAs(path2);
                        path2 = "/Images/docs/" + Path.GetFileName(model.Id + model.Fic_photo.FileName);
                        collaborateurPhoto.Fic_autorisation_photo = path1;
                        collaborateurPhoto.Fic_photo = path2;
                        model.Fic_autorisation_photoCollab = path1;
                        model.Fic_photoCollab = path2;
                        if (dal.MiseAJourPhoto(collaborateurPhoto))
                        {
                            model.Fic_autorisation_photoCollab = collaborateurPhoto.Fic_autorisation_photo;
                            model.Fic_photoCollab = collaborateurPhoto.Fic_photo;
                            ajaxListViewModel.successMsg = Resources.Resources.FileSaved;
                            model.Fic_photo_path = path2;
                            ModelState.Clear();
                            ajaxListViewModel.html = RenderRazorViewToString("InsertPhoto", model);
                        }
                        else
                        {
                            ajaxListViewModel.errorMsg = Resources.Resources.ProblemImport;
                        }
                    }
                    catch (Exception ex)
                    {
                        model.Erreur = "Erreur" + ex.Message;
                        ajaxListViewModel.errorMsg = model.Erreur;
                    }
                }
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        #endregion
        #region PhotoVue Suppression en mode Post
        /// <summary>
        /// Importer la photo        
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult PhotoVueDelete(Photo model)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            if (!dal.PhotoExiste(model.Id))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.AlreadyDelete;
            }
            else
            {
                Collaborateur collaborateurPhoto = new Collaborateur();
                collaborateurPhoto.Id = model.Id;
                collaborateurPhoto = dal.ObtientPhotoDetail(collaborateurPhoto.Id);
                model.Fic_autorisation_photoCollab = collaborateurPhoto.Fic_autorisation_photo;
                model.Fic_photoCollab = collaborateurPhoto.Fic_photo;
                collaborateurPhoto.Fic_autorisation_photo = null;
                collaborateurPhoto.Fic_photo = null;
                if (dal.MiseAJourPhoto(collaborateurPhoto))
                {
                    SupprimerChemin(model.Fic_photoCollab);
                    SupprimerChemin(model.Fic_autorisation_photoCollab);
                    model.Fic_autorisation_photoCollab = collaborateurPhoto.Fic_autorisation_photo;
                    model.Fic_photoCollab = collaborateurPhoto.Fic_photo;
                    ajaxListViewModel.successMsg = Resources.Resources.FilesDelete;
                    model.Fic_photo_path = null;
                }
                else
                {
                    ajaxListViewModel.errorMsg = Resources.Resources.ErrorDelete;
                }
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        public void SupprimerChemin(string model)
        {
            var chemin = RecupererChemin(model);
            if (System.IO.File.Exists(chemin))
            {
                System.IO.File.Delete(chemin);
            }
        }
        public string RecupererChemin(string model)
        {
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = Path.Combine(contentRootPath, "Images\\docs");
            var path1 = model.Trim();
            string str = "";
            for (int i = 13; i <= path1.Length - 1; i++)
            {
                str += path1[i];
            }
            return Path.Combine(path + str);
        }
        #endregion
        #endregion
        #region Gestion urgence
        #region UrgenceVue en mode Get
        /// <summary>
        /// Appel vue UrgenceVue en mode Get pour pouvoir lancer le modal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddUrgenceModale(Collaborateur model)
        {
            Urgence urgenceInput = new Urgence();
            urgenceInput.FK_id_collaborateur = model.Id;
            return PartialView("_AddUrgenceModale", urgenceInput);
        }
        public ActionResult UpdateUrgenceView(Collaborateur model)
        {
            Urgence urgenceInput = new Urgence();
            urgenceInput.FK_id_collaborateur = model.Id;
            return PartialView("UpdateUrgence", urgenceInput);
        }

        #endregion
        #region Affichage des tous les urgences déjà créer creation d'une ViewData
        /// <summary>
        /// Récuperation des tous les urgences déjà existantes pour le collaborateur
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public ActionResult ListUrgence(Collaborateur detail)
        {
            Urgence urgenceInput = new Urgence();
            urgenceInput.FK_id_collaborateur = detail.Id;
            ChargerListUrgences(urgenceInput);
            return PartialView(urgenceInput);
        }

        #endregion
        #region UrgenceVue en mode Post
        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult UrgenceVue(Urgence detail)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            // test de validation des champs
            if ((!ModelState.IsValid) ||
                 !TestChampUrgence(detail))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.MandatoryInput + " " + Resources.Resources.NotSufficient;
            }
            else
            {
                Urgence newUrg = dal.CreerUrgence(detail);
                if (newUrg != null)
                {
                    ajaxListViewModel.successMsg = Resources.Resources.FileSaved;
                    ChargerListUrgences(detail);
                    ModelState.Clear();
                    ajaxListViewModel.html = RenderRazorViewToString("UrgRow", newUrg);
                }
                else
                {
                    ChargerListUrgences(detail);
                    ajaxListViewModel.errorMsg = Resources.Resources.ProblemEmergency;
                }
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        #region Controle du validite des champs d'entree urgence
        public static bool TestChampUrgence(Urgence detail)
        {
            if (string.IsNullOrWhiteSpace(detail.Adresse) ||
                 string.IsNullOrWhiteSpace(detail.Lien) ||
                 string.IsNullOrWhiteSpace(detail.Nom) ||
                 string.IsNullOrWhiteSpace(detail.Prenom) ||
                 string.IsNullOrWhiteSpace(detail.Tel_fixe) ||
                 string.IsNullOrWhiteSpace(detail.Tel_port) ||
                 detail.Nom.Length < 3 ||
                 detail.Nom.Length > 30 ||
                 detail.Prenom.Length < 3 ||
                 detail.Prenom.Length > 30 ||
                 detail.Adresse.Length < 3 ||
                 detail.Adresse.Length > 150 ||
                 detail.Tel_fixe.Length < 10 ||
                 detail.Tel_fixe.Length > 20 ||
                 detail.Tel_port.Length < 10 ||
                 detail.Tel_port.Length > 20 ||
                 detail.Lien.Length < 3 ||
                 detail.Lien.Length > 20)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult UrgenceUpdateVue(Urgence detail)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            // test de validation des champs
            if ((!ModelState.IsValid) ||
                 !TestChampUrgence(detail))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.MandatoryInput + " " + Resources.Resources.NotSufficient;
            }
            else
            {
                if (dal.MiseAJourUrgence(detail))
                {
                    ajaxListViewModel.successMsg = Resources.Resources.FilesModified;
                    ChargerListUrgences(detail);
                    ModelState.Clear();
                    ajaxListViewModel.html = RenderRazorViewToString("UrgRow", detail);
                }
                else
                {
                    ChargerListUrgences(detail);
                    ajaxListViewModel.errorMsg = Resources.Resources.ProblemEmergency;
                }
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }


        public ActionResult DeleteUrgenceView(Collaborateur model)
        {

            Urgence urgenceInput = new Urgence();
            urgenceInput.FK_id_collaborateur = model.Id;
            return PartialView("DeleteUrgence", urgenceInput);
        }



        [HttpPost]
        public ActionResult UrgenceDeleteVue(Urgence detail)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();

            if (dal.RecupererUrgence(detail.Id))
            {
                if (dal.DeleteUrgence(detail))
                {
                    ajaxListViewModel.successMsg = Resources.Resources.ContactDelete;
                    ChargerListUrgences(detail);
                    ModelState.Clear();
                    ajaxListViewModel.html = RenderRazorViewToString("UrgRow", detail);

                }
                else
                {
                    ChargerListUrgences(detail);
                    ajaxListViewModel.errorMsg = Resources.Resources.ProblemEmergency;
                }
            }
            else
            {
                ChargerListUrgences(detail);
                ajaxListViewModel.errorMsg = Resources.Resources.ContactAlreadyDelete;
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        #endregion
        #endregion
        #region Fonctions communs pour modale
        /*public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }*/
        public async Task<string> RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult =
                     _viewEngine.FindView(ControllerContext, viewName, false);

                ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                /*viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);*/
                return sw.GetStringBuilder().ToString();
            }
        }
        /*-------------------------------------------------------------------*/
        public void ChargerTousLesModals(Collaborateur model)
        {
            Urgence urgenceInput = new Urgence();
            urgenceInput.FK_id_collaborateur = model.Id;
            ChargerListUrgences(urgenceInput);
            ChargerListLangues(model);
            ChargerListTechniques(model);
            ChargerListEtudes(model);
            Experiences experienceInput = new Experiences();
            experienceInput.FK_id_collaborateur = model.Id;
            ChargerListDomaines(experienceInput);
            ChargerListExperiences(experienceInput);
        }

        public void ChargerListUrgences(Urgence detail)
        {
            List<Urgence> urgenceList = dal.RecupererUrgences(detail.FK_id_collaborateur);
            IList<Urgence> urgenceInput = new List<Urgence>();
            urgenceInput = urgenceList;
            ViewData["listeUrgences"] = urgenceInput;
        }
        public void ChargerListExperiences(Experiences detail)
        {
            List<Poste> posteList = dal.RecupererExperiences(detail.FK_id_collaborateur);
            List<Experiences> liste = new List<Experiences>();
            foreach (var p in posteList)
            {
                Experiences poste = new Experiences(p.Id, p.FK_id_collaborateur, p.Nom_client, p.Description, p.Date_debut, p.Date_fin, p.Intitule, p.Ville, p.Pays, p.Charge, p.Logo, p.Domaine);
                liste.Add(poste);
            }
            IList<Experiences> posteInput = new List<Experiences>();
            posteInput = liste;
            ViewData["listeExperiences"] = posteInput;
        }
        #endregion
        #region Skills
        public ActionResult SkillsTab(Collaborateur collab)
        {
            ChargerListLangues(collab);
            ChargerListTechniques(collab);
            ChargerListEtudes(collab);
            return PartialView("SkillsTab", collab);
        }
        public void ChargerListLangues(Collaborateur model)
        {
            /////////////////////////////////////////////////////////
            //Données pour LANGUE
            /////////////////////////////////////////////////////////
            //liste toutes les langues "neutres" i.e non associées à un pays (sinon on aurait eu par exemple 36x arabe pour arabe maroc, tunisie, égypte etc...)
            var languages = CultureInfo.GetCultures(CultureTypes.NeutralCultures)
                            .Select(x => x.DisplayName)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList();
            ViewBag.languages = new SelectList(languages);

            Dictionary<int, string> listeNiveau = new Dictionary<int, string>();
            listeNiveau.Add(1, "1: Notions élémentaire");
            listeNiveau.Add(2, "2: Compétences professionnelles limitées");
            listeNiveau.Add(3, "3: Compétences professionnelles");
            listeNiveau.Add(4, "4: Compétences professionnelles complètes");
            listeNiveau.Add(5, "5: Bilingue ou langue natale");
            ViewData["langues"] = dal.RecupererCompetences(model.Id, "L");
            ViewBag.listeNiveau = new SelectList(listeNiveau.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text");
        }
        public ActionResult ChargerLanguesCollab(Collaborateur model)
        {
            ViewData["langues"] = dal.RecupererCompetences(model.Id, "L");
            Langues langModel = new Langues();
            langModel.FK_id_collaborateur = model.Id;
            return PartialView(langModel);
        }
        public void ChargerListTechniques(Collaborateur model)
        {
            /////////////////////////////////////////////////////////
            //Données pour COMPTETENCES TECHNIQUES
            /////////////////////////////////////////////////////////
            Dictionary<int, string> listeNiveauTech = new Dictionary<int, string>();
            listeNiveauTech.Add(1, "1: Notions");
            listeNiveauTech.Add(2, "2: Utilisateur");
            listeNiveauTech.Add(3, "3: Confirmé");
            listeNiveauTech.Add(4, "4: Expert");
            ViewBag.listeNiveauTech = new SelectList(listeNiveauTech.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text");
            ViewData["tech"] = dal.RecupererCompetences(model.Id, "T");
        }
        public void ChargerListFonctionnelle(Collaborateur model)
        {
            /////////////////////////////////////////////////////////
            //Données pour COMPTETENCES FONCTIONNELLES
            /////////////////////////////////////////////////////////

            Dictionary<int, string> listeNiveauFonc = new Dictionary<int, string>();
            listeNiveauFonc.Add(1, "1: Notions");
            listeNiveauFonc.Add(2, "2: Utilisateur");
            listeNiveauFonc.Add(3, "3: Confirmé");
            listeNiveauFonc.Add(4, "4: Expert");

            ViewBag.listeNiveauFonc = new SelectList(listeNiveauFonc.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text");
            /////////////////////////////////////////////////////////
            //Données pour MODIFICATION
            /////////////////////////////////////////////////////////
            Dictionary<string, string> listSkillType = new Dictionary<string, string>();
            listSkillType.Add("L", "Langue");
            listSkillType.Add("O", "Formation");
            listSkillType.Add("T", "Technique");
            listSkillType.Add("F", "Fonctionnel");
            ViewBag.listSkillType = new SelectList(listSkillType.Select(x => new { Value = x.Key, Text = x.Value }), "Value", "Text");

            ViewData["fonct"] = dal.RecupererCompetences(model.Id, "F");
        }
        public void ChargerListEtudes(Collaborateur model)
        {
            ViewData["form"] = dal.RecupererEtudes(model.Id);
        }

        // Ajout de langue => modale et Post du form
        public ActionResult AddLangSkillModale(Collaborateur model)
        {
            ChargerListLangues(model);
            Langues langModel = new Langues();
            langModel.FK_id_collaborateur = model.Id;
            return PartialView("AddLangSkill", langModel);
        }
        [HttpPost]
        public ActionResult AddLangSkill(Langues model)
        {
            Competence comp = new Competence();
            if (model.FK_id_collaborateur != 0)
            {
                string path = string.Empty;
                if (model.UploadFile != null && model.UploadFile.Length > 0)
                    try
                    {
                        string contentRootPath = _webHostEnvironment.ContentRootPath;
                        path = Path.Combine(contentRootPath, "Images");
                        path = Path.Combine(path,
                                                   Path.GetFileName(model.UploadFile.FileName));
                        model.UploadFile.SaveAs(path);
                        ViewBag.Message = "Le fichier a été télécharger avec succès ! ";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERREUR:" + ex.Message.ToString();
                    }
                //Maintenant on va ajouter la skill saisie
                comp = dal.AddLangSkill(model.FK_id_collaborateur, model.Langue, "L", model.Niveau, path);
                ViewBag.Message = "Form submitted.";

            }
            return PartialView("Skill", comp);
        }

        // Ajout de langue => modale et Post du form
        public ActionResult EditLangSkillModale(Collaborateur model)
        {
            ChargerListLangues(model);
            Langues langModel = new Langues();
            langModel.FK_id_collaborateur = model.Id;
            return PartialView("EditLangSkill", langModel);
        }
        [HttpPost]
        public ActionResult EditLangSkill(Competence model)
        {
            Competence comp = new Competence();
            comp = dal.EditLangSkill(model.IdCollab, model.Id, model.Nom, model.Type, model.Niveau, null);
            ViewBag.Message = "Form submitted.";
            return PartialView("Skill", comp);
        }

        public ActionResult ListLangues(Collaborateur model)
        {
            ChargerListLangues(model);
            Langues langModel = new Langues();
            langModel.FK_id_collaborateur = model.Id;
            return PartialView(langModel);
        }
        public ActionResult AddTechSkillModale(Collaborateur model)
        {
            ChargerListTechniques(model);
            Techniques techModel = new Techniques();
            techModel.FK_id_collaborateur = model.Id;
            return PartialView("AddTechSkill", techModel);
        }
        public ActionResult ListeTechniques(Collaborateur model)
        {
            ChargerListTechniques(model);
            Techniques techModel = new Techniques();
            techModel.FK_id_collaborateur = model.Id;
            return PartialView(techModel);
        }
        [HttpPost]
        public ActionResult AddTechSkill(Techniques model)
        {
            Competence comp = new Competence();
            if (model.FK_id_collaborateur != 0)
            {
                comp = dal.AddSkill(model.FK_id_collaborateur, model.TechSkill.ToUpper(), "T", model.TechNiveau);
                ViewBag.Message = "Form submitted.";
            }
            return PartialView("Skill", comp);
        }
        public ActionResult AddFoncSkillModale(Collaborateur model)
        {
            ChargerListFonctionnelle(model);
            Fonctions foncModel = new Fonctions();
            foncModel.FK_id_collaborateur = model.Id;
            return PartialView("AddFoncSkill", foncModel);
        }
        public ActionResult ListeFonctionnelles(Collaborateur model)
        {
            ChargerListFonctionnelle(model);
            Fonctions foncModel = new Fonctions();
            foncModel.FK_id_collaborateur = model.Id;
            return PartialView(foncModel);
        }
        [HttpPost]
        public ActionResult AddFoncSkill(Fonctions model)
        {
            Competence comp = new Competence();
            if (model.FK_id_collaborateur != 0)
            {
                comp = dal.AddSkill(model.FK_id_collaborateur, model.FuncSkill.ToUpper(), "O", model.FuncNiveau);
                ViewBag.Message = "Form submitted.";
            }
            return PartialView("Skill", comp);
        }
        public ActionResult AddFormationModale(Collaborateur model)
        {
            ChargerListEtudes(model);
            Formations formModel = new Formations();
            formModel.FK_id_collaborateur = model.Id;
            return PartialView("AddFormation", formModel);
        }
        public ActionResult ListeEtudes(Collaborateur model)
        {
            ChargerListEtudes(model);
            Formations formModel = new Formations();
            formModel.FK_id_collaborateur = model.Id;
            return PartialView(formModel);
        }
        [HttpPost]
        public ActionResult AddFormation(Formations model)
        {
            Etude formation = new Etude();
            if (model.FK_id_collaborateur != 0)
            {
                string path = string.Empty;
                if (model.UploadFile != null && model.UploadFile.Length > 0)
                    try
                    {
                        string contentRootPath = _webHostEnvironment.ContentRootPath;
                        path = Path.Combine(contentRootPath, "Images");
                        path = Path.Combine(path,
                                                   Path.GetFileName(model.UploadFile.FileName));
                        model.UploadFile.SaveAs(path);
                        ViewBag.Message = "Le fichier a été télécharger avec succès ! ";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERREUR:" + ex.Message.ToString();
                    }
                formation = dal.AddEtude(
                   model.FK_id_collaborateur,
                   model.Intitule,
                   model.Ville,
                   model.Domaine,
                   model.Etablissement,
                   path,
                   model.DateDeb,
                   model.DateFin
                   );
                ViewBag.Message = "Form submitted.";
            }
            return PartialView("SkillEtude", formation);
        }
        #endregion       
        #region Function pour tester si date est valide
        /// <summary>
        /// Verification que la date n'est pas plus petit que le siecle 1800
        /// et que c'est vraiment une date
        /// on return true si date est valide
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool TestDate(DateTime date)
        {
            // on n'accepte pas les date plus petit que le siecle 1800
            string test2 = date.ToString("yyyyMMdd");
            int result = string.Compare("18000101", test2);

            if (result > 0)
            {
                return false;
            }
            else
            {
                // contrôle date 
                string testdate = Convert.ToString(date);
                string testdate1 = testdate.Substring(0, 10);
                Regex regex = new Regex(@"(((0|1)[0-9]|2[0-9]|3[0-1])\/(0[1-9]|1[0-2])\/((19|20)\d\d))$");
                //Verify whether entered date itestdates Valid date.
                return DateTime.TryParseExact(testdate1, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out _);
            }
        }
        #endregion
        #region Gestion experience
        #region ExperienceVue en mode Get
        /// <summary>
        /// Appel vue _AddExperienceModale en mode Get pour pouvoir lancer le modal
        /// Charger la liste des tous les domaines possibles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult AddExperienceModale(Collaborateur model)
        {
            Experiences experienceInput = new Experiences();
            experienceInput.FK_id_collaborateur = model.Id;
            experienceInput.Charge = 100;
            experienceInput.Date_debut = DateTime.Now;
            experienceInput.Date_fin = DateTime.Now;
            ChargerListDomaines(experienceInput);
            return PartialView("_AddExperienceModale", experienceInput);
        }
        /// <summary>
        /// Appel vue UpdateExperience en mode Get pour pouvoir lancer le modal
        /// Charger la liste des tous les domaines possibles
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult UpdateExperienceView(Collaborateur model)
        {
            Experiences experienceInput = new Experiences();
            experienceInput.FK_id_collaborateur = model.Id;
            experienceInput.Charge = 100;
            experienceInput.Date_debut = DateTime.Now;
            experienceInput.Date_fin = DateTime.Now;
            experienceInput.LogoClient = "/Images/docs/image.jpg";
            ChargerListDomaines(experienceInput);
            return PartialView("UpdateExperience", experienceInput);
        }
        /// <summary>
        /// Récuperer tous les experiences déjà créer trier par date fin
        /// Appel vue partial ListExperience en mode Get pour afficher tous les experiences
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public ActionResult ListExperience(Collaborateur detail)
        {
            Experiences experienceInput = new Experiences();
            experienceInput.FK_id_collaborateur = detail.Id;
            ChargerListExperiences(experienceInput);

            return PartialView(experienceInput);
        }
        public ActionResult DeleteExperienceView(Collaborateur model)
        {
            Experiences experienceInput = new Experiences();
            experienceInput.FK_id_collaborateur = model.Id;
            experienceInput.Charge = 100;
            experienceInput.Date_debut = DateTime.Now;
            experienceInput.Date_fin = DateTime.Now;
            experienceInput.LogoClient = "/Images/docs/image.jpg";
            ChargerListDomaines(experienceInput);

            return PartialView("DeleteExperience", experienceInput);
        }
        /// <summary>
        /// Création liste de tous les domaines
        /// </summary>
        /// <param name="model"></param>
        public void ChargerListDomaines(Experiences model)
        {
            /////////////////////////////////////////////////////////
            //Données pour Experience domaines
            /////////////////////////////////////////////////////////
            List<string> listeDomaine = new List<string>();
            listeDomaine.Add(Resources.Resources.Area1);
            listeDomaine.Add(Resources.Resources.Area2);
            listeDomaine.Add(Resources.Resources.Area3);
            listeDomaine.Add(Resources.Resources.Area4);
            listeDomaine.Add(Resources.Resources.Area5);
            listeDomaine.Add(Resources.Resources.Area6);
            listeDomaine.Add(Resources.Resources.Area7);
            listeDomaine.Add(Resources.Resources.Area8);
            listeDomaine.Add(Resources.Resources.Area9);
            listeDomaine.Add(Resources.Resources.Area10);
            listeDomaine.Add(Resources.Resources.Area11);
            listeDomaine.Add(Resources.Resources.Area12);
            listeDomaine.Add(Resources.Resources.Area13);
            listeDomaine.Add(Resources.Resources.Area14);
            listeDomaine.Add(Resources.Resources.Area15);

            ViewBag.listeDomaine = new SelectList(listeDomaine, selectedValue: model.Domaine);
        }
        #endregion
        #region ExperienceVue en mode Post
        #region Nouveau Experience en mode Post
        /// <summary>
        /// Récuperation experience saisit dans _AddExperienceModale
        /// Contrôle de la saisie 
        /// (validation des champs,contrôle date debut et date fin et contrôle cohérence date debut et date fin  )
        /// création path si logo est saisie  
        /// création nouveau poste et mise à jour de la date fin mission du collaborateur   
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult ExperienceVue(Experiences detail)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();

            // test de validation des champs
            if ((!ModelState.IsValid) ||
                 !TestChampPoste(detail))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.MandatoryInput + " " + Resources.Resources.NotSufficient;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }
            // contrôle date debut et date fin   
            if (!TestDate(detail.Date_debut) || !TestDate(detail.Date_fin))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.DateError;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }
            // contrôle cohérence date debut et date fin  
            if (!TestCoherence(detail))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.DateErrorC;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }

            string charge = Convert.ToString(detail.Charge);
            if (!Int32.TryParse(charge, out int _) || string.IsNullOrWhiteSpace(charge) || detail.Charge > 100)
            {
                ajaxListViewModel.errorMsg = Resources.Resources.InvalidAvail;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }
            // création path si logo est saisie   
            if (detail.Logo != null)
            {
                string path1 = string.Empty;
                try
                {
                    string contentRootPath = _webHostEnvironment.ContentRootPath;
                    string path = Path.Combine(contentRootPath, "Images\\docs");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    int id = dal.DernierIdPoste();
                    id++;
                    path1 = Path.Combine(path, Path.GetFileName(id + detail.Logo.FileName));
                    detail.Logo.SaveAs(path1);
                    path1 = "/Images/docs/" + Path.GetFileName(id + detail.Logo.FileName);
                    detail.LogoClient = path1;
                }
                catch (Exception ex)
                {
                    ajaxListViewModel.errorMsg = ex.Message;
                }
            }
            // création nouveau poste et mise à jour de la date fin mission du collaborateur   
            Poste newPoste = PreparePoste(detail);

            newPoste = dal.CreerPoste(newPoste);

            if (newPoste != null && MiseAJourCollab(newPoste))
            {
                ajaxListViewModel.successMsg = Resources.Resources.FileSaved;
                detail.Id = newPoste.Id;
                ChargerListExperiences(detail);
                ModelState.Clear();
                ajaxListViewModel.html = RenderRazorViewToString("PostRow", detail);
            }
            else
            {
                ChargerListExperiences(detail);
                ajaxListViewModel.errorMsg = Resources.Resources.ProblemPoste;
            }

            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        #endregion
        #region Mise à jour date fin mission dans la table collaborateur
        /// <summary>
        /// Récuperation du poste avec la date de fin le plus grand
        /// mettre à jour le fichier collaborateur avec la date et charge trouvé
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool MiseAJourCollab(Poste model)
        {
            Poste modelMax = dal.DateFinMax(model.FK_id_collaborateur);
            Collaborateur detail = new Collaborateur();
            detail.Id = model.FK_id_collaborateur;

            if (modelMax == null)
            {
                detail.Date_fin_mission = null;
                detail.Charge = 100;
            }
            else
            {
                detail.Date_fin_mission = modelMax.Date_fin;
                detail.Charge = modelMax.Charge;
            }

            if (dal.MiseAJourFinMissionCollab(detail))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region Controle du validite des champs d'entree experience
        public static bool TestChampPoste(Experiences detail)
        {
            if (string.IsNullOrWhiteSpace(detail.Nom_client) ||
                 string.IsNullOrWhiteSpace(detail.Description) ||
                 string.IsNullOrWhiteSpace(detail.Intitule) ||
                 string.IsNullOrWhiteSpace(detail.Ville) ||
                 string.IsNullOrWhiteSpace(detail.Pays) ||
                 string.IsNullOrWhiteSpace(detail.Domaine) ||
                 detail.Nom_client.Length < 3 ||
                 detail.Nom_client.Length > 50 ||
                 detail.Description.Length < 3 ||
                 detail.Intitule.Length < 3 ||
                 detail.Intitule.Length > 100 ||
                 detail.Ville.Length < 3 ||
                 detail.Ville.Length > 50 ||
                 detail.Pays.Length < 3 ||
                 detail.Pays.Length > 50 ||
                 detail.Domaine.Length < 3 ||
                 detail.Domaine.Length > 50)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
        #region contrôle cohérence date debut et date fin  
        public static bool TestCoherence(Experiences detail)
        {
            string test1 = detail.Date_debut.ToString("yyyyMMdd");
            string test2 = detail.Date_fin.ToString("yyyyMMdd");
            int result = string.Compare(test1, test2);
            if (result > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
        #region Initialiser Poste
        public static Poste PreparePoste(Experiences detail)
        {
            Poste newPoste = new Poste();
            newPoste.Intitule = detail.Intitule;
            newPoste.FK_id_collaborateur = detail.FK_id_collaborateur;
            newPoste.Nom_client = detail.Nom_client;
            newPoste.Pays = detail.Pays;
            newPoste.Ville = detail.Ville;
            newPoste.Domaine = detail.Domaine;
            newPoste.Date_debut = detail.Date_debut;
            newPoste.Date_fin = detail.Date_fin;
            newPoste.Logo = detail.LogoClient;
            newPoste.Charge = detail.Charge;
            newPoste.Description = detail.Description;
            return newPoste;
        }
        #endregion
        #region Modification Experience en mode Post
        /// <summary>
        /// Récuperation experience modifier dans UpdateExperienceModale
        /// Contrôle de la saisie 
        /// (validation des champs,contrôle date debut et date fin et contrôle cohérence date debut et date fin  )
        /// modification path si logo est saisie et suppression de l'ancien path si il y a 
        /// modification poste et mise à jour de la date fin mission du collaborateur   
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult ExperienceUpdateVue(Experiences detail)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            // test de validation des champs
            if ((!ModelState.IsValid) ||
                !TestChampPoste(detail))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.MandatoryInput + " " + Resources.Resources.NotSufficient;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }
            // contrôle date debut et date fin   
            if (!TestDate(detail.Date_debut) || !TestDate(detail.Date_fin))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.DateError;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }
            // contrôle cohérence date debut et date fin  
            if (!TestCoherence(detail))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.DateErrorC;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }
            string charge = Convert.ToString(detail.Charge);
            if (!Int32.TryParse(charge, out int _) || string.IsNullOrWhiteSpace(charge) || detail.Charge > 100)
            {
                ajaxListViewModel.errorMsg = Resources.Resources.InvalidAvail;
                return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
            }
            Poste posteAvant = new Poste();
            posteAvant.Id = detail.Id;
            posteAvant = dal.ObtientPosteDetail(detail.Id);
            // modification poste et mise à jour de la date fin mission du collaborateur 
            Poste newPoste = PreparePoste(detail);
            newPoste.Id = detail.Id;
            newPoste.FK_id_collaborateur = detail.FK_id_collaborateur;
            // modification path si logo est saisie   
            if (detail.Logo != null)
            {
                string path1 = string.Empty;
                try
                {
                    string contentRootPath = _webHostEnvironment.ContentRootPath;
                    string path = Path.Combine(contentRootPath, "Images\\docs");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    if (posteAvant.Logo != null)
                    {
                        SupprimerChemin(posteAvant.Logo);
                    }
                    path1 = Path.Combine(path, Path.GetFileName(detail.Id + detail.Logo.FileName));
                    detail.Logo.SaveAs(path1);
                    path1 = "/Images/docs/" + Path.GetFileName(detail.Id + detail.Logo.FileName);
                    detail.LogoClient = path1;
                    newPoste.Logo = path1;
                    // mise à jour path si logo est saisie  
                    if (!dal.MiseAJourLogo(newPoste))
                    {
                        ajaxListViewModel.errorMsg = Resources.Resources.ProblemPoste;
                    }
                }
                catch (Exception ex)
                {
                    ajaxListViewModel.errorMsg = ex.Message;
                }
            }
            else
            {
                detail.LogoClient = posteAvant.Logo;
                newPoste.Logo = posteAvant.Logo;
            }
            if (dal.MiseAJourPoste(newPoste) && MiseAJourCollab(newPoste))
            {
                ajaxListViewModel.successMsg = Resources.Resources.FilesModified;
                detail.Id = newPoste.Id;
                ChargerListExperiences(detail);
                ModelState.Clear();
                ajaxListViewModel.html = RenderRazorViewToString("PostRow", detail);
            }
            else
            {
                ChargerListExperiences(detail);
                ajaxListViewModel.errorMsg = Resources.Resources.ProblemPoste;
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        #endregion
        #region Suppression Experience en mode Post
        /// <summary>
        /// Récuperation experience suppression dans ExperiencDeleteVueModale
        /// suppression du path si il y a 
        /// suppression poste et mise à jour de la date fin mission du collaborateur   
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExperienceDeleteVue(Experiences detail)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            if (dal.RecupererPoste(detail.Id))
            {
                Poste posteAvant = new Poste();
                posteAvant.Id = detail.Id;
                posteAvant = dal.ObtientPosteDetail(detail.Id);
                if (posteAvant.Logo != null)
                {
                    SupprimerChemin(posteAvant.Logo);
                }
                if (dal.DeletePoste(posteAvant) && MiseAJourCollab(posteAvant))
                {
                    ajaxListViewModel.successMsg = Resources.Resources.PosteDelete;
                    ChargerListExperiences(detail);
                    ModelState.Clear();
                    ajaxListViewModel.html = RenderRazorViewToString("PostRow", detail);
                }
                else
                {
                    ChargerListExperiences(detail);
                    ajaxListViewModel.errorMsg = Resources.Resources.ProblemPoste;
                }
            }
            else
            {
                ChargerListExperiences(detail);
                ajaxListViewModel.errorMsg = Resources.Resources.PosteAlreadyDelete;
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        #endregion
        #endregion
        #endregion
        /// <summary>
        /// Affichage de la vue de recherche de collaborateur
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchCollab(string q)
        {
            return View();
        }

        /// <summary>
        /// Recherche des collaborateurs selon leur nom ou prénom pour autocomplétion
        /// </summary>
        /// <param name="prefix">début du nom ou prénom recherché</param>
        /// <returns>Tableau Json contenant les collaborateurs correspondants</returns>
        public JsonResult AjaxSearchCollab(string prefix)
        {
            List<Collaborateur> collaborateurs = dal.RechercheCollaborateur(prefix);
            return Json(new
            {
                collaborateurs,
                noResults = Resources.Resources.NoResults
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        /// <summary>
        /// Affichage de la liste des collaborateurs
        /// </summary>
        /// <param name="query">nom prénom recherché</param>
        /// <param name="ordre">ordre d'affichage : 1 = alphabétique, 2 = dernier ajout</param>
        /// <param name="dispo">dispo : 0 = Tout, 1 = disponible, 2 = 3 mois ou moins, 3 = indisponible</param>
        /// <param name="other_agency">autre agence, 1 si on cherche les autres agences</param>
        /// <returns>Retourne la vue liste des collaborateurs</returns>
        [HttpPost]
        public ActionResult SearchCollab(string query, int[] dispo, int ordre = 0, int charge = 1, int other_agency = 0, int without_dispo = 0)
        {
            List<Collaborateur> collaborateurs = dal.RechercheCollaborateur(query, dispo, ordre, charge, other_agency);
            if (without_dispo == 0)
                ViewData["background"] = true;
            return PartialView("_CollaboratorsList", collaborateurs);
        }

        /// <summary>
        /// Affichage de la vue de recherche de collaborateur
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchCompet(string[] q)
        {
            return View();
        }

        /// <summary>
        /// Recherche des collaborateurs selon leur nom ou prénom pour autocomplétion
        /// </summary>
        /// <param name="prefix">début du nom ou prénom recherché</param>
        /// <returns>Tableau Json contenant les collaborateurs correspondants</returns>
        public JsonResult AjaxSearchCompet(string prefix)
        {
            List<string> competences = dal.RechercheCompetences(prefix);
            return Json(new
            {
                competences,
                noResults = Resources.Resources.NoResults
            }/*, JsonRequestBehavior.AllowGet*/);
        }

        /// <summary>
        /// Affichage de la liste des collaborateurs selon leurs compétences
        /// </summary>
        /// <param name="query">compétences recherchées</param>
        /// <param name="dispo">dispo : 0 = Tout, 1 = disponible, 2 = 3 mois ou moins, 3 = indisponible</param>
        /// <param name="other_agency">autre agence, 1 si on cherche les autres agences</param>
        /// <returns>Retourne la vue liste des collaborateurs</returns>
        [HttpPost]
        public ActionResult SearchCompet(string query, int[] dispo, int other_agency = 0)
        {
            string queryNorm = query.Replace(" ", "");
            if (queryNorm.Last() == ',')
                queryNorm = queryNorm.Substring(0, queryNorm.Length - 1);

            string[] queryTab = queryNorm.Split(',');
            ViewData["header"] = queryTab;
            List<CollabCompet> collaborateurs = dal.RechercheCollaborateurWithCompet(queryTab, dispo, other_agency);
            return PartialView("_CollabCompetList", collaborateurs);
        }

        /// <summary>
        /// Affichage de la vue de suivi de collaborateur
        /// </summary>
        /// <returns></returns>
        public ActionResult CollabMonitSearch()
        {
            return View();
        }
        #region Affichage de la vue du collaborateur
        /// <summary>
        /// Affichage de la vue du collaborateur
        /// </summary>
        /// <returns></returns>
        public ActionResult DispCollab(int? id)
        {
            // recupérer le collaborateur si il existe
            if (id.HasValue)
            {
                Collaborateur collaborateur = dal.RecupererCollaborateur(id.Value);
                if (collaborateur != null)
                {
                    if (TestAccess(collaborateur))
                    {
                        List<Competence> tousLesLangues = dal.RecupererCompetences(collaborateur.Id, "L");
                        List<Etude> tousLesEtudes = dal.RecupererEtudes(collaborateur.Id);
                        List<Formation> tousLesFormations = dal.RecupererFormations(collaborateur.Id);
                        List<Competence> tousLesCompTechniques = dal.RecupererCompetences(collaborateur.Id, "T");
                        List<Competence> tousLesCompFonctionnelles = dal.RecupererCompetences(collaborateur.Id, "F");
                        List<Urgence> tousLesUrgences = dal.RecupererUrgences(collaborateur.Id);
                        Photo photo = new Photo();
                        photo.Fic_photo_path = collaborateur.Fic_photo;
                        Poste posteMax = new Poste();
                        posteMax.Nom_client = " ";
                        posteMax.Intitule = " ";
                        posteMax.Ville = " ";
                        posteMax.Pays = " ";
                        if (collaborateur.Date_fin_mission != null)
                        {
                            posteMax = dal.DateFinMax(collaborateur.Id);
                        }
                        List<Poste> tousLesPostes = dal.RecupererExperiences(collaborateur.Id);
                        CollaDis nouveauCollaEdit = new CollaDis(collaborateur, photo, tousLesLangues, tousLesCompTechniques, tousLesCompFonctionnelles, tousLesEtudes, tousLesFormations, tousLesUrgences, posteMax, tousLesPostes);
                        return View(nouveauCollaEdit);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    string messageTitre = Resources.Resources.SeriousError;
                    string messageErreur = Resources.Resources.Reason + Resources.Resources.Reason2;
                    string commentaireErreur = Resources.Resources.NotifyAdmin;
                    ErreurGrave nouveauErreur = new ErreurGrave(messageTitre, messageErreur, commentaireErreur);
                    return RedirectToAction("ErreurGrave", "Collaborator", new { nouveauErreur });
                }
            }
            else
            {
                string messageTitre = Resources.Resources.SeriousError;
                string messageErreur = Resources.Resources.Reason + Resources.Resources.Reason2;
                string commentaireErreur = Resources.Resources.NotifyAdmin;
                ErreurGrave nouveauErreur = new ErreurGrave(messageTitre, messageErreur, commentaireErreur);
                return RedirectToAction("ErreurGrave", "Collaborator", new { nouveauErreur });
            }
        }
        /// <summary>
        /// Profil administrateur (AD)a tous les access sur tous les collaborateurs des tous les agences
        /// Profil responsable agence (DA) a tous les access sur tous les collaborateurs des son agence
        /// Profil commercial (CO) a tous les access sur tous les collaborateurs des son agence
        /// Profil commercial (RH) a tous les access sur tous les collaborateurs sauf sur gestion collaboratuer des son agence
        /// Profil responsable agence (DA) a seulement access de lecture de tous les collaborateurs des autres agences 
        /// il n'a pas droit d'editer archiver ni acces sur formation et gestion collaborateur
        /// Pour tous les autres cas en retourne au menu acceuil
        /// </summary>
        /// <param name="collaborateur"></param>
        /// <returns></returns>
        public bool TestAccess(Collaborateur collaborateur)
        {
            int idUtil = int.Parse(HttpContext.User.Identity.Name);
            string profil = dal.RecupererProfil(idUtil);
            if (profil == "AD")
            {
                ViewData["access"] = 1;
                return true;
            }
            else
            {
                if (dal.TestAccessAgence(collaborateur.FK_id_agence, idUtil))
                {
                    if (profil == "DA" || profil == "CO")
                    {
                        ViewData["access"] = 1;
                        return true;
                    }
                    else
                    {
                        if (profil == "RH")
                        {
                            ViewData["access"] = 2;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (profil == "DA")
                    {
                        ViewData["access"] = 3;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        #endregion
        #region Archivage du collaborateur
        public ActionResult ArchiveView(Collaborateur collab)
        {
            if (!collab.Date_fin_contrat.HasValue)
            {
                collab.Date_fin_contrat = DateTime.Now;
            }
            int result = TestAFaireArchive(collab);
            if (result > 0)
            {
                ViewData["Archivage"] = true;
            }
            else
            {
                ViewData["Archivage"] = false;
            }

            return PartialView("ArchiveModal", collab);
        }

        public ActionResult _ArchiveModal(Collaborateur collab)
        {
            if (collab.Date_fin_contrat.HasValue)
            {
                ViewBag.dateFinContrat = collab.Date_fin_contrat.Value.ToString("yyyy-MM-dd ");
            }
            else
            {
                ViewBag.dateFinContrat = DateTime.Now.ToString("yyyyMMdd");
            }
            return PartialView("_ArchiveModal", collab);
        }
        [ValidateAntiForgeryToken]

        [HttpPost]
        public ActionResult ArchiveCollab(Collaborateur model)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            // contrôle date de fin contrat   

            if (!model.Date_fin_contrat.HasValue || !TestDate(model.Date_fin_contrat.Value))
            {
                ajaxListViewModel.errorMsg = Resources.Resources.ErrorContratDate;
            }
            else
            {
                model.Archive = true;
                if (dal.MiseAJourArchive(model, out model))
                {
                    ajaxListViewModel.successMsg = Resources.Resources.DateContractRecorded;
                    ModelState.Clear();
                    ajaxListViewModel.html = RenderRazorViewToString("InfoFinContrat", model);
                }
                else
                {
                    ajaxListViewModel.errorMsg = Resources.Resources.ErrorTableContract;
                }
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        public static int TestAFaireArchive(Collaborateur model)
        {
            string test1 = DateTime.Now.ToString("yyyyMMdd");
            string test2 = model.Date_fin_contrat.Value.ToString("yyyyMMdd");
            return string.Compare(test1, test2);

        }
        #endregion
        #region Function pour enlever les blanc des champs du collaborateur
        /// <summary>
        /// Function en entrée un collaborateur et il enleve tous les blancs avant et arrière
        /// </summary>
        /// <param name="collaborateur"></param>
        /// <returns></returns>
        public static Collaborateur TrimCollaborateur(Collaborateur collaborateur)

        {
            collaborateur.Nom = collaborateur.Nom.Trim();
            collaborateur.Prenom = collaborateur.Prenom.Trim();

            collaborateur.Adresse = collaborateur.Adresse.Trim();
            collaborateur.Code_postal = collaborateur.Code_postal.Trim();
            collaborateur.Ville = collaborateur.Ville.Trim();
            collaborateur.Pays = collaborateur.Pays.Trim();

            collaborateur.Email = collaborateur.Email.Trim();
            collaborateur.Email_travail = collaborateur.Email_travail.Trim();
            collaborateur.Tel = collaborateur.Tel.Trim();
            collaborateur.Tel_travail = collaborateur.Tel_travail.Trim();
            return collaborateur;
        }
        #endregion

        public ActionResult ExportView(Collaborateur collab)
        {
            return PartialView("_Export", collab);
        }

        /// <summary>
        /// Exportation selon choix de l'utilisateur
        /// </summary>
        /// <param name="idsToExport"></param>
        /// <param name="target"></param>
        /// <param name="nom"></param>
        /// <param name="initiales"></param>
        /// <param name="poste"></param>
        /// <param name="formations"></param>
        /// <param name="certifications"></param>
        /// <param name="langues"></param>
        /// <param name="experiences"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Export(int[] idsToExport, int target, int nom = 0, int initiales = 0, int poste = 0, int formations = 0, int certifications = 0,
            int langues = 0, int experiences = 0)
        {
            bool isPdf = false;
            string filename = string.Empty;
            object oMissing = Type.Missing;

            // création du document, et initialisation d'un paragraphe
            Application app = new Application();
            Document document = app.Documents.Add();
            var para = document.Paragraphs.Add();

            int count = 1;
            // boucle d'ecriture des collaborateurs
            foreach (int id in idsToExport)
            {
                Collaborateur collab = dal.RecupererCollaborateur(id);
                DocumentHelper.createDocument(para, collab, nom, initiales, poste, formations, certifications, langues, experiences, dal);
                // on fait un saut de ligne sauf au dernier collab
                if (count < idsToExport.Length)
                    document.Words.Last.InsertBreak();
                count++;
            }

            // creation dossier si non existant
            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = Path.Combine(contentRootPath, "Ressources\\docs");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Sauvegarde du fichier
            document.SaveAs(Path.Combine(path, "export.docx"));

            // Si l'utilisateur veut un pdf, on exporte le doc en pdf
            if (target == 2)
            {
                document.ExportAsFixedFormat(Path.Combine(path, "export.pdf"), WdExportFormat.wdExportFormatPDF);
                filename = "export.pdf";
                isPdf = true;
            }
            else
                filename = "export.docx";

            // Fermeture du document
            document.Close();
            app.Quit();

            // Récupération du document en byte[]
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(path, filename));
            // écriture du fichier de retour
            var file = File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

            // Suppression des fichiers
            System.IO.File.Delete(Path.Combine(path, "export.docx"));
            if (isPdf)
                System.IO.File.Delete(Path.Combine(path, "export.pdf"));

            return file;
        }

        public ActionResult ImportModale(Collaborateur collab)
        {
            return PartialView("_Import", collab);
        }

        public ActionResult Import(int idCollab, IFormFile file)
        {
            AjaxViewModel ajaxListViewModel = new AjaxViewModel();
            string path = string.Empty;
            Collaborateur model = dal.RecupererCollaborateur(idCollab);
            if (file != null && file.Length > 0)
            {
                try
                {
                    // creation dossier si non existant
                    string contentRootPath = _webHostEnvironment.ContentRootPath;
                    path = Path.Combine(contentRootPath, "Ressources\\docs");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    path = Path.Combine(path,
                                                Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    DocumentHelper.genereCollab(model, dal, path);
                    System.IO.File.Delete(Path.Combine(path, file.FileName));
                    ChargerListLangues(model);
                    ChargerListTechniques(model);
                    ChargerListEtudes(model);
                    ajaxListViewModel.successMsg = "L'importation s'est bien passée, verifiez bien les informations importées";
                    ajaxListViewModel.html = RenderRazorViewToString("SkillsTab", model);
                    //ajaxListViewModel.html2 = RenderRazorViewToString("ExpTab", model);
                }
                catch (Exception ex)
                {
                    ajaxListViewModel.errorMsg = "Problème avec le fichier";
                    ViewBag.Message = "ERREUR:" + ex.Message.ToString();
                }
            }
            return Json(ajaxListViewModel/*, JsonRequestBehavior.AllowGet*/);
        }
        [HttpGet]
        public JsonResult SearchDomaine(string prefix)
        {

            List<string> listeDomaine = new List<string>();
            if (Resources.Resources.Area1.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area1);
            }
            if (Resources.Resources.Area2.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area2);
            }
            if (Resources.Resources.Area3.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area3);
            }
            if (Resources.Resources.Area4.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area4);
            }
            if (Resources.Resources.Area5.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area5);
            }
            if (Resources.Resources.Area6.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area6);
            }
            if (Resources.Resources.Area7.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area7);
            }
            if (Resources.Resources.Area8.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area8);
            }
            if (Resources.Resources.Area9.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area9);
            }
            if (Resources.Resources.Area10.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area10);
            }
            if (Resources.Resources.Area11.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area11);
            }
            if (Resources.Resources.Area12.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area12);
            }
            if (Resources.Resources.Area13.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area13);
            }
            if (Resources.Resources.Area14.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area14);
            }
            if (Resources.Resources.Area15.ToUpper().Contains(prefix.ToUpper()))
            {
                listeDomaine.Add(Resources.Resources.Area15);
            }

            return Json(new
            {
                listeDomaine,
                noResults = Resources.Resources.NoResults
            }/*, JsonRequestBehavior.AllowGet*/);
        }
    }
}