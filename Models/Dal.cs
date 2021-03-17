using Apogee.Data;
using Apogee.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Apogee.Models
{
    public class Dal : IDal
    {
        private ApplicationDbContext bdd;
        /// <summary>
        /// Constructeur de la Database Access Layer
        /// </summary>
        public Dal(ApplicationDbContext db)
        {
            bdd = db;
        }
               
        #region Collaborateur

        /// <summary>
        /// 
        /// Récuperation du collaborateur d'id donné
        /// </summary>
        /// <param name="id">id du collaborateur recherché</param>
        /// <returns>collaborateur recherché ou null</returns>
        public Collaborateur RecupererCollaborateur(int id)
        {
            return bdd.Collaborateurs.First(m => m.Id == id);
        }

        /// <summary>
        /// Recherche de collaborateur selon leur nom, prénom ou "nom prénom"
        /// </summary>
        /// <param name="query">nom et/ou prénom recherché</param>
        /// <returns>Liste des collaborateurs correspondants</returns>
        public List<Collaborateur> RechercheCollaborateur(string query)
        {
            var firstWord = query.Split(' ')[0];
            var secondWord = query.Split(' ').Length > 1 ? query.Split(' ')[1] : "";
            return bdd.Collaborateurs.Where(m =>
               m.Nom.ToLower().StartsWith(query.ToLower()) ||
               m.Prenom.ToLower().StartsWith(query.ToLower()) ||
                (m.Nom.ToLower() == firstWord.ToLower() && m.Prenom.ToLower().StartsWith(secondWord.ToLower())) ||
                (m.Prenom.ToLower() == firstWord.ToLower() && m.Nom.ToLower().StartsWith(secondWord.ToLower()))
               ).ToList();
        }

        /// <summary>
        /// Recherche de collaborateur avec filtres
        /// </summary>
        /// <param name="query">nom et/ou prénom recherché</param>
        /// <param name="dispo">disponibilité, 1 : disponible, 2 : dans 3 mois ou moins, 3 : indisponible</param>
        /// <param name="ordre">ordre d'affichage, 1 : alphabétique, 2 : derniers ajouts</param>
        /// <param name="charge">charge de travail à 100% ou non</param>
        /// <param name="other_agency">id d'une autre agence pour les directeurs d'agence</param>
        /// <returns>Liste des collaborateurs correspondants</returns>
        public List<Collaborateur> RechercheCollaborateur(string query, int[] dispo, int ordre, int charge, int other_agency = 0)
        {
            var firstWord = query.Split(' ')[0];
            var secondWord = query.Split(' ').Length > 1 ? query.Split(' ')[1] : "";
            var dbCollab = bdd.Collaborateurs.Where(m =>
               m.Nom.ToLower().StartsWith(query.ToLower()) ||
               m.Prenom.ToLower().StartsWith(query.ToLower()) ||
                (m.Nom.ToLower() == firstWord.ToLower() && m.Prenom.ToLower().StartsWith(secondWord.ToLower())) ||
                (m.Prenom.ToLower() == firstWord.ToLower() && m.Nom.ToLower().StartsWith(secondWord.ToLower()))
               );
            // tri selon la disponibilité
            DateTime today3month = DateTime.Now.AddMonths(3);
            if (dispo != null && dispo.Contains(1))
                dbCollab = dbCollab.Where(m => m.Date_fin_mission == null || m.Date_fin_mission.Value <= DateTime.Now);
            if (dispo != null && dispo.Contains(2))
                dbCollab = dbCollab.Where(m => today3month >= m.Date_fin_mission.Value);
            if (dispo != null && dispo.Contains(3))
                dbCollab = dbCollab.Where(m => today3month < m.Date_fin_mission.Value);

            // tri selon la charge, si != 0 : seulement les charges à 100%
            if (charge != 0)
                dbCollab = dbCollab.Where(m => m.Charge == 100);

            // ordre d'affichage
            if (ordre == 1) // alphabétique
                dbCollab = dbCollab.OrderBy(m => m.Nom);
            else if (ordre == 2) // derniers ajouts
                dbCollab = dbCollab.OrderByDescending(m => m.Id);

            //TODO : gestion des agences (agence de l'utilisateur connecté + autres agences éventuelles)

            return dbCollab.ToList();
        }
        #endregion

        #region Utilisateur
        /// <summary>
        /// Authentification de l'utilisateur avec son email/mot de passe
        /// </summary>
        /// <param name="email">email de l'utilisateur se connectant</param>
        /// <param name="motDePasse">mot de passe entré</param>
        /// <returns>Utilisateur correspondant, null si l'authentification échoue</returns>
        public LoginReturnViewModel Authentifier(string email, string motDePasse)
        {
            LoginReturnViewModel returnViewModel = new LoginReturnViewModel();
            Utilisateur utilisateur = RechercheUtilisateur(email);
            if (utilisateur.ValiditeMDP.HasValue && utilisateur.ValiditeMDP.Value.CompareTo(DateTime.Now) <= 0)
                returnViewModel.PasswordInValidity = false; // validite du mot de passe
            else
                returnViewModel.PasswordInValidity = true;

            if (utilisateur.DateBloquage.HasValue && utilisateur.DateBloquage.Value.CompareTo(DateTime.Now) >= 0)
                returnViewModel.TimeBlocked = utilisateur.DateBloquage.Value.Subtract(DateTime.Now).TotalMinutes; // ajout en réponse du temps de bloquage restant

            //Si l'utilisateur existe et qu'il n'est pas bloqué
            if (utilisateur != null && returnViewModel.PasswordInValidity && returnViewModel.TimeBlocked == 0)
            {
                // utilisation du sha2-256
                using (SHA256 hash = SHA256.Create())
                {
                    // récupération du mot de passe en base de donnée
                    var password = utilisateur.MotDePasse;
                    var secure_password_salted = password.Split('_')[0];
                    var salt = password.Split('_')[1].Trim();

                    // hashage du mot de passe entré
                    StringBuilder Sb = new StringBuilder();
                    Encoding enc = Encoding.UTF8;
                    Byte[] result = hash.ComputeHash(enc.GetBytes(motDePasse + salt));

                    // concaténation des bytes du mot de passe hashé
                    foreach (Byte b in result)
                        Sb.Append(b.ToString("x2"));

                    // vérification du mot de passe entré
                    var password_sha = Sb.ToString();
                    if (password_sha.ToUpper().Equals(secure_password_salted.ToUpper()))
                    {
                        utilisateur.CompteurMDPFaux = 0; // on réinitialise le nombre d'erreur
                        utilisateur.DateBloquage = null; // on réinitialise la date de bloquage
                        bdd.SaveChanges();               // on sauvegarde la modification
                        returnViewModel.Utilisateur = utilisateur;
                    }
                    else
                    {
                        utilisateur.CompteurMDPFaux += 1;  // on incrémente le nombre d'echec de mot de passe
                        returnViewModel.NbErrorsLeft = 3 - utilisateur.CompteurMDPFaux; // ajout en réponse du nombre d'erreur restantes
                        if (utilisateur.CompteurMDPFaux == 3)
                        {
                            utilisateur.DateBloquage = DateTime.Now.AddHours(1); // on ajoute une heure à la date de bloquage
                            utilisateur.CompteurMDPFaux = 0;                     // on réinitialise le nombre d'erreur
                            returnViewModel.TimeBlocked = utilisateur.DateBloquage.Value.Subtract(DateTime.Now).TotalMinutes; // ajout en réponse du temps de bloquage restant
                        }
                        bdd.SaveChanges(); // on sauvegarde la modification
                    }
                }
            }
            return returnViewModel;
        }

        /// <summary>
        /// Changement de mot de passe
        /// </summary>
        /// <param name="id">id de l'utilisateur</param>
        /// <param name="motDePasse">nouveau mot de passe</param>
        /// <returns>nouveau mot de passe hashé</returns>
        public string ChangePassword(int id, string motDePasse)
        {
            Utilisateur utilisateur = RecupererUtilisateur(id);
            using (SHA256 hash = SHA256.Create())
            {
                //////////////////////////////////////////////PARTIE SALT
                var random = new Random();
                // Maximum length of salt
                int max_length = 32;
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                // Return the string encoded salt
                string rslt = new string(Enumerable.Repeat(chars, max_length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
                //////////////////////////////////////////////PARTIE CONCATENATION
                string spw = motDePasse + rslt;
                //////////////////////////////////////////////PARTIE HASHAGE
                StringBuilder Sb = new StringBuilder();
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(spw));
                // concaténation des bytes du mot de passe hashé
                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
                //////////////////////////////////////////////RESULTAT + STOCKAGE BDD
                var password_sha = Sb.ToString();
                var final_password = password_sha + "_" + rslt;
                utilisateur.MotDePasse = final_password;
                bdd.SaveChanges();
                return final_password;
            }
        }

        /// <summary>
        /// Réinitialisation du mot de passe de l'utilisateur dont l'email est fourni
        /// </summary>
        /// <param name="email">Email de l'utilisateur dont on réinitialise le mot de passe</param>
        /// <returns>Nouveau mot de passe</returns>
        public string ResetPassword(string email)
        {
            // récupération de l'utilisateur
            Utilisateur utilisateur = RechercheUtilisateur(email);
            // génération d'un nouveau mot de passe
            string password = /*Membership.GeneratePassword(16, 2);*/ "password";
            ChangePassword(utilisateur.Id, password);

            // on ajoute une heure pour la validité
            utilisateur.ValiditeMDP = DateTime.Now.AddHours(1);
            bdd.SaveChanges();

            return password;
        }

        /// <summary>
        /// Réinitialisation de la période de validité d'un mot de passe temporaire
        /// </summary>
        /// <param name="utilisateur">Utilisateur dont on réinitialise la validité</param>
        public void ResetValiditeMdp(Utilisateur utilisateur)
        {
            utilisateur.ValiditeMDP = null;
            bdd.SaveChanges();
        }

        /// <summary>
        /// Recherche d'utilisateur a partir de son email
        /// </summary>
        /// <param name="email">adresse email de l'utilisateur recherché</param>
        /// <returns>Utilisateur correspondant, null si non trouvé</returns>
        public Utilisateur RechercheUtilisateur(string email)
        {
            return bdd.Utilisateurs.FirstOrDefault(m =>
               m.Email.ToLower().StartsWith(email.ToLower())
               );
        }

        /// <summary>
        /// Recherche d'utilisateur a partir de son email
        /// </summary>
        /// <param name="id">id de l'utilisateur recherché</param>
        /// <returns>Utilisateur correspondant, null si non trouvé</returns>
        public Utilisateur RecupererUtilisateur(int? id)
        {
            /* if (id != null)
             {
                 return bdd.Utilisateurs.FirstOrDefault(m => m.Id == id);
             }
             else
             {
                 return new Utilisateur()
                 {
                     Prenom = "Floriant",
                     Nom = "Fekete",
                     Email = "floflo@sfr.fr",
                     MotDePasse = "coucou",
                 };
             }*/
            string name = bdd.Utilisateurs.FirstOrDefault(m => m.Id == id).Nom;
            return bdd.Utilisateurs.FirstOrDefault(m => m.Id == id);
        }

        #region Accèss à la base de donnéé concernant la création du collaborateur
        /// <summary>
        /// Recuperation des tous les agences du profil utilisateur
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Agence> ObtientTousLesAgences(int id)
        {
            List<Agence> resultats = new List<Agence>();
            List<Affect_Utilisateur_Agence> resultatsProv = ObtientTousLesIdAgences(id);
            foreach (var resultatsDetail in resultatsProv)
            {
                resultats.Add(ObtenirAgence(resultatsDetail.fk_id_agence));
            }
            return resultats;
        }
        /// <summary>
        /// Recuperation agence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Agence ObtenirAgence(int id)
        {
            return bdd.Agences.Where(a => a.Id.Equals(id)).FirstOrDefault();
        }
        /// <summary>
        /// Recuperation première agence possible d'utilisateur pour création par defaut
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int ObtenirFirstAgence(int id)
        {
            return bdd.Affect_Utilisateur_Agences.Where(u => u.fk_id_utilisateur.Equals(id)).Select(p => p.fk_id_agence).FirstOrDefault();

        }
       

        /// <summary>
        ///  On compte les nombres de different pays de l'utilisateur
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public int ObtenirPaysAgence(List<Agence> detail)
        {
            int count = 0;          
            foreach (IGrouping<string, Agence> grouping in detail.GroupBy(v => v.Code_pays))
            {            
                count++;
            }
            return count;
        }
       /// <summary>
       /// Récuperation des tous les contrats par pays selectionné
       /// </summary>
       /// <param name="pays"></param>
       /// <returns></returns>
        public List<Contrat> ObtientTousLesContrats(string pays)
        {

            return bdd.Contrats.Where(c =>
                     c.Code_pays.Equals(pays)).ToList();
        }
        /// <summary>
        /// Recuperation premier contrat possible d'utilisateur pour création par defaut
        /// </summary>
        /// <param name="pays"></param>
        /// <returns></returns>
        public int ObtenirFirstContrat(string pays)
        {
            return bdd.Contrats.Where(u => u.Code_pays.Equals(pays)).Select(p => p.Id).FirstOrDefault();

        }
        /// <summary>
        /// Vérification que le contrat et le pays correspond bien
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code_pays"></param>
        /// <returns></returns>
        public bool ContratExiste(int id, string code_pays)
        {
            if (bdd.Contrats.Count(u => u.Id == id & u.Code_pays == code_pays) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Recuperation premier statut possible d'utilisateur pour création par defaut
        /// </summary>
        /// <param name="pays"></param>
        /// <returns></returns>
        public int ObtenirFirstStatut(string pays)
        {
            return bdd.Statut_Contrats.Where(u => u.Code_pays.Equals(pays)).Select(p => p.Id).FirstOrDefault();

        }
        /// <summary>
        /// Vérification que le statut et le pays correspond bien
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code_pays"></param>
        /// <returns></returns>
        public bool StatutExiste(int id, string code_pays)
        {
            if (bdd.Statut_Contrats.Count(u => u.Id == id & u.Code_pays == code_pays) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Vérification que le statut et le pays correspond bien
        /// </summary>
        /// <param name="id"></param>
        /// <param name="code_pays"></param>
        /// <returns></returns>
        public bool NiveauExiste(int id, string code_pays)
        {
            if (bdd.Niveaux.Count(u => u.Id == id & u.Code_pays == code_pays) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Recuperation premier statut possible d'utilisateur pour création par defaut
        /// </summary>
        /// <param name="pays"></param>
        /// <returns></returns>
        public int ObtenirFirstNiveau(string pays)
        {
            return bdd.Niveaux.Where(u => u.Code_pays.Equals(pays)).Select(p => p.Id).FirstOrDefault();

        }
        /// <summary>
        /// Vérification que le statut et le pays correspond bien
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fK_id_niveau"></param>
        /// <returns></returns>
        public bool CoefficientExiste(int id, int fK_id_niveau)
        {
            if (bdd.Coefficients.Count(u => u.Id == id & u.FK_id_niveau == fK_id_niveau) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Recuperation premier coefficient possible d'utilisateur pour création par defaut
        /// </summary>
        /// <param name="fK_id_niveau"></param>
        /// <returns></returns>
        public int ObtenirFirstCoefficient(int fK_id_niveau)
        {
            return bdd.Coefficients.Where(u => u.FK_id_niveau.Equals(fK_id_niveau)).Select(p => p.Id).FirstOrDefault();

        }
        /// <summary>
        /// Recuperation premier commercial com = "CO" possible d'utilisateur pour création par defaut
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        public int ObtientFirstCommercial(int id, string comm)
        {
            var entryPoint = (from ep in bdd.Affect_Utilisateur_Agences
                              join e in bdd.Utilisateurs on ep.fk_id_utilisateur equals e.Id

                              where e.Profil == comm & ep.fk_id_agence == id
                              select new
                              {
                                  Id = e.Id,
                                  Nom = e.Nom,
                                  Prenom = e.Prenom

                              }).FirstOrDefault();
            return entryPoint.Id;


        }
        /// <summary>
        /// Récuperation des tous les statuts par pays selectionné
        /// </summary>
        /// <param name="pays"></param>
        /// <returns></returns>
        public List<Statut_Contrat> ObtientTousLesStatutContrat(string pays)
        {

            return bdd.Statut_Contrats.Where(c =>
                     c.Code_pays.Equals(pays)).ToList();
        }
        /// <summary>
        /// Récuperation des tous les niveaux par pays selectionné
        /// </summary>
        /// <param name="pays"></param>
        /// <returns></returns>
        public List<Niveau> ObtientTousLesNiveaux(string pays)
        {

            return bdd.Niveaux.Where(c =>
                     c.Code_pays.Equals(pays)).ToList();
        }
        /// <summary>
        /// Récuperation des tous les coefficients si niveau a changé
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Coefficient> ObtientTousLesCoefficients(int id)
        {
            return bdd.Coefficients.Where(c =>
                    c.FK_id_niveau.Equals(id)).ToList();
        }       
        /// <summary>
        /// Récuperation des tous les agences de l'utilisateur
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Affect_Utilisateur_Agence> ObtientTousLesIdAgences(int id)
        {
            return bdd.Affect_Utilisateur_Agences.Where(m =>
                     m.fk_id_utilisateur.Equals(id)).ToList();
        }
        /// <summary>
        /// Récuperation des tous les commerciales par agence selectionné
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        public List<Utilisateur> ObtientTousLesCommercials(int id, string comm)
        {
            var entryPoint = (from ep in bdd.Affect_Utilisateur_Agences
                              join e in bdd.Utilisateurs on ep.fk_id_utilisateur equals e.Id

                              where e.Profil == comm & ep.fk_id_agence == id
                              select new
                              {
                                  Id = e.Id,
                                  Nom = e.Nom,
                                  Prenom = e.Prenom

                              }).ToList();
            List<Utilisateur> liste = new List<Utilisateur>();
            foreach (var p in entryPoint)
            {
                Utilisateur commercial = new Utilisateur(p.Id, p.Nom, p.Prenom);
                liste.Add(commercial);
            }
            return liste;

        }
        /// <summary>
        /// Vérification que le commercial existe bien
        /// </summary>
        /// <param name="fK_id_agence"></param>
        /// <param name="fK_id_commercial"></param>
        /// <returns></returns>
        public bool CommercialExiste(int fK_id_agence, int fK_id_commercial)
        {

            if (bdd.Affect_Utilisateur_Agences.Count(u => u.fk_id_utilisateur == fK_id_commercial & u.fk_id_agence == fK_id_agence) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Vérification que le matricule n'est pas en double
        /// </summary>
        /// <param name="matricule"></param>
        /// <returns></returns>
        public bool MatriculeExiste(int matricule, int id)
        {
            if (bdd.Collaborateurs.Count(u => u.Matricule == matricule & u.Id != id) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Vérification si le nom et prénom existe déjà
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="prenom"></param>
        /// <returns></returns>
        public bool NomPrenomExiste(string nom, string prenom)
        {
            if (bdd.Collaborateurs.Count(u => u.Nom == nom & u.Prenom == prenom) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Récuperation du suffixe maximal si le nom et prénom existe déjà
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="prenom"></param>
        /// <returns></returns>
        public int NomPrenomMax(string nom, string prenom)
        {
            return bdd.Collaborateurs.Where(u => u.Nom.Equals(nom) && u.Prenom.Equals(prenom)).Select(p => p.Suffixe_homonyme).DefaultIfEmpty(0).Max();

        }
        /// <summary>
        /// Récuperation le pays de l'agence selectionné
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string PaysAgence(int id)
        {
            return bdd.Agences.Where(u => u.Id.Equals(id)).Select(p => p.Code_pays).FirstOrDefault();

        }
        /// <summary>
        /// Création d'un nouveau collaborateur
        /// </summary>
        /// <param name="collaborateur"></param>
        /// <returns></returns>
        public bool CreerCollaborateur(Collaborateur collaborateur)
        {
            try
            {
                bdd.Collaborateurs.Add(new Collaborateur
                {
                    Abilite_defense = collaborateur.Abilite_defense,
                    Matricule = collaborateur.Matricule,
                    Civilite = collaborateur.Civilite,
                    Nom = collaborateur.Nom.Trim(),
                    Prenom = collaborateur.Prenom.Trim(),
                    FK_id_agence = collaborateur.FK_id_agence,
                    Adresse = collaborateur.Adresse.Trim(),
                    Code_postal = collaborateur.Code_postal.Trim(),
                    Ville = collaborateur.Ville.Trim(),
                    Pays = collaborateur.Pays.Trim(),
                    Date_naissance = collaborateur.Date_naissance,
                    Email = collaborateur.Email,
                    Email_travail = collaborateur.Email_travail,
                    Tel = collaborateur.Tel.Trim(),
                    Tel_travail = collaborateur.Tel_travail.Trim(),
                    FK_id_statut = collaborateur.FK_id_statut,
                    FK_id_contrat = collaborateur.FK_id_contrat,
                    FK_id_niveau = collaborateur.FK_id_niveau,
                    FK_id_coefficient = collaborateur.FK_id_coefficient,
                    Salaire_brut_mens = collaborateur.Salaire_brut_mens,
                    Date_embauche = collaborateur.Date_embauche,
                    Salaire_bru_ann = collaborateur.Salaire_bru_ann,
                    Suffixe_homonyme = collaborateur.Suffixe_homonyme,
                    FK_id_commercial = collaborateur.FK_id_commercial,
                    Charge = collaborateur.Charge

                });
                bdd.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// Mise à jour du Collaborateur
        /// </summary>
        /// <param name="collaborateur"></param>
        /// <returns></returns>
        public bool MiseAJourCollab(Collaborateur collaborateur)
        {
            try
            {
                Collaborateur collabTrouve = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == collaborateur.Id);
                if (collabTrouve != null)
                {
                    collabTrouve.Abilite_defense = collaborateur.Abilite_defense;
                    collabTrouve.Matricule = collaborateur.Matricule;
                    collabTrouve.Civilite = collaborateur.Civilite;
                    collabTrouve.Nom = collaborateur.Nom.Trim();
                    collabTrouve.Prenom = collaborateur.Prenom.Trim();
                    collabTrouve.FK_id_agence = collaborateur.FK_id_agence;
                    collabTrouve.Adresse = collaborateur.Adresse.Trim();
                    collabTrouve.Code_postal = collaborateur.Code_postal.Trim();
                    collabTrouve.Ville = collaborateur.Ville.Trim();
                    collabTrouve.Pays = collaborateur.Pays.Trim();
                    collabTrouve.Date_naissance = collaborateur.Date_naissance;
                    collabTrouve.Email = collaborateur.Email.Trim();
                    collabTrouve.Email_travail = collaborateur.Email_travail.Trim();
                    collabTrouve.Tel = collaborateur.Tel.Trim();
                    collabTrouve.Tel_travail = collaborateur.Tel_travail.Trim();
                    collabTrouve.FK_id_statut = collaborateur.FK_id_statut;
                    collabTrouve.FK_id_contrat = collaborateur.FK_id_contrat;
                    collabTrouve.FK_id_niveau = collaborateur.FK_id_niveau;
                    collabTrouve.FK_id_coefficient = collaborateur.FK_id_coefficient;
                    collabTrouve.Salaire_brut_mens = collaborateur.Salaire_brut_mens;
                    collabTrouve.Date_embauche = collaborateur.Date_embauche;
                    collabTrouve.Salaire_bru_ann = collaborateur.Salaire_bru_ann;
                    collabTrouve.Suffixe_homonyme = collaborateur.Suffixe_homonyme;
                    collabTrouve.FK_id_commercial = collaborateur.FK_id_commercial;
                    collabTrouve.Charge = collaborateur.Charge;
                    bdd.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }

        }
        public Urgence CreerUrgence(Urgence detail)
        {
            try
            {
                bdd.Urgences.Add(detail);
                bdd.SaveChanges();
                return detail;
            }
            catch
            {
                return null;
            }
        }
        public bool DeleteUrgence(Urgence detail)
        {
            try
            {
                var urgence = bdd.Urgences.Single(o => o.Id == detail.Id);
                bdd.Urgences.Remove(urgence);

                bdd.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }
        public bool MiseAJourUrgence(Urgence detail)
        {
            try
            {
                Urgence detailTrouve = bdd.Urgences.FirstOrDefault(urgence => urgence.Id == detail.Id);
                if (detailTrouve != null)
                {

                    detailTrouve.Nom = detail.Nom.Trim();
                    detailTrouve.Prenom = detail.Prenom.Trim();
                    detailTrouve.Adresse = detail.Adresse.Trim();
                    detailTrouve.Tel_fixe = detail.Tel_fixe.Trim();
                    detailTrouve.Tel_port = detail.Tel_port.Trim();
                    detailTrouve.Tel_travail = detail.Tel_travail.Trim();
                    detailTrouve.Lien = detail.Lien.Trim();
                    bdd.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public Collaborateur ObtientPhotoDetail(int id)
        {
            var entryPoint = (from ep in bdd.Collaborateurs
                              where ep.Id == id
                              select new
                              {
                                  Id = ep.Id,
                                  Fic_autorisation_photo = ep.Fic_autorisation_photo,
                                  Fic_photo = ep.Fic_photo
                              }).FirstOrDefault();
            Collaborateur collaborateur = new Collaborateur();
            collaborateur.Id = entryPoint.Id;
            collaborateur.Fic_autorisation_photo = entryPoint.Fic_autorisation_photo;
            collaborateur.Fic_photo = entryPoint.Fic_photo;
            return collaborateur;
        }
        /// <summary>
        /// Mise à jour du Collaborateur
        /// </summary>
        /// <param name="collaborateur"></param>
        /// <returns></returns>
        public bool MiseAJourPhoto(Collaborateur collaborateur)
        {
            try
            {
                Collaborateur collabTrouve = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == collaborateur.Id);
                if (collabTrouve != null)
                {
                    collabTrouve.Fic_autorisation_photo = collaborateur.Fic_autorisation_photo;
                    collabTrouve.Fic_photo = collaborateur.Fic_photo;
                    bdd.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }

        }
        public bool PhotoExiste(int id)
        {
            try
            {
                Collaborateur collabTrouve = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == id);
                if (collabTrouve != null)
                {
                    if (collabTrouve.Fic_autorisation_photo != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }

        }
        #endregion
        #region Accèss à la base de donnéé concernant l'affichage du collaborateur
        /// <summary>
        ///  Recuperation dernier id d'un collaborateur
        /// </summary>
        /// <param name="collaborateur"></param>
        /// <returns></returns>
        public int DernierIdCollab(Collaborateur collaborateur)
        {
            return bdd.Collaborateurs.Where(u => u.Matricule.Equals(collaborateur.Matricule) && u.Email.Equals(collaborateur.Email) && u.Email_travail.Equals(collaborateur.Email_travail) && u.Nom.Equals(collaborateur.Nom)).Select(u => u.Id).DefaultIfEmpty(0).Max();
        }

        /// <summary>
        /// Recuperation des competences par type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public List<Competence> RecupererCompetences(int id, string comp)
        {
            return bdd.Competences.Where(m =>
                     m.IdCollab.Equals(id) & m.Type.Equals(comp)).ToList();
        }
        /// <summary>
        /// Recuperation des formations d'un utlisateur
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public List<Etude> RecupererEtudes(int id)
        {
            return bdd.Etudes.Where(m =>
                     m.IdCollab.Equals(id)).ToList();
        }
        /// <summary>
        /// Recuperation des formations d'un utlisateur
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public List<Formation> RecupererFormations(int id)
        {
            return bdd.Formations.Where(m =>
                     m.FK_id_collaborateur.Equals(id) & m.Validation.Equals(true)).ToList();
        }
        /// <summary>
        /// Recuperation des urgences  d'un utlisateur
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public List<Urgence> RecupererUrgences(int id)
        {
            var entryPoint = (from ep in bdd.Urgences

                              where ep.FK_id_collaborateur == id
                              select new
                              {
                                  Id = ep.Id,
                                  Nom = ep.Nom,
                                  Prenom = ep.Prenom,
                                  Tel_fixe = ep.Tel_fixe,
                                  Tel_port = ep.Tel_port,
                                  Tel_travail = ep.Tel_travail,
                                  Adresse = ep.Adresse,
                                  FK_id_collaborateur = ep.FK_id_collaborateur,
                                  Lien = ep.Lien

                              }).ToList();
            List<Urgence> liste = new List<Urgence>();
            foreach (var p in entryPoint)
            {
                Urgence urgence = new Urgence(p.Id, p.FK_id_collaborateur, p.Nom, p.Prenom, p.Adresse, p.Tel_fixe, p.Tel_port, p.Tel_travail, p.Lien);
                liste.Add(urgence);
            }
            return liste;

        }
        public bool RecupererUrgence(int id)
        {

            if (bdd.Urgences.Count(u => u.Id == id) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// Recuperation des urgences  d'un utlisateur
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public List<Poste> RecupererExperiences(int id)
        {
         
            List<Poste> liste = new List<Poste>();

            var entryPoint = (from ep in bdd.Postes

                              where ep.FK_id_collaborateur == id
                              orderby ep.Date_fin
                              select new
                              {
                                  Id = ep.Id,
                                  Nom_client = ep.Nom_client,
                                  Description = ep.Description,
                                  Date_debut = ep.Date_debut,
                                  Date_fin = ep.Date_fin,
                                  Intitule = ep.Intitule,
                                  Ville = ep.Ville,
                                  FK_id_collaborateur = ep.FK_id_collaborateur,
                                  Pays = ep.Pays,
                                  Charge = ep.Charge,
                                  Logo = ep.Logo,
                                  Domaine = ep.Domaine
                              }).ToList();

            foreach (var p in entryPoint)    
            {      
                Poste poste = new Poste(p.Id, p.FK_id_collaborateur, p.Nom_client, p.Description, p.Date_debut, p.Date_fin, p.Intitule, p.Ville, p.Pays, p.Charge, p.Logo, p.Domaine);
                liste.Add(poste);               
            }
            return liste;
        }
        public Poste CreerPoste(Poste detail)
        {
            try
            {
                bdd.Postes.Add(detail);
                bdd.SaveChanges();
                return detail;
            }
            catch
            {
                return null;
            }
        }
        public int DernierIdPoste()
        {
            return bdd.Postes.Select(u => u.Id).DefaultIfEmpty(0).Max();
        }
        public Poste DateFinMax(int id)
        {
            return bdd.Postes.Where(u => u.FK_id_collaborateur.Equals(id)).OrderByDescending(m => m.Date_fin).FirstOrDefault();
        }
        public Poste ObtientPosteDetail(int id)
        {
            return bdd.Postes.Where(a => a.Id.Equals(id)).FirstOrDefault();           
        }
        public bool MiseAJourPoste(Poste detail)
        {
            try
            {
                Poste detailTrouve = bdd.Postes.FirstOrDefault(poste => poste.Id == detail.Id);
                if (detailTrouve != null)
                {

                    detailTrouve.Nom_client = detail.Nom_client.Trim();
                    detailTrouve.Description = detail.Description.Trim();
                    detailTrouve.Date_debut = detail.Date_debut;
                    detailTrouve.Date_fin = detail.Date_fin;
                    detailTrouve.Intitule = detail.Intitule.Trim();
                    detailTrouve.Ville = detail.Ville.Trim();
                    detailTrouve.Pays = detail.Pays.Trim();
                    detailTrouve.Charge = detail.Charge;                   
                    detailTrouve.Domaine = detail.Domaine.Trim();                   
                    bdd.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool MiseAJourLogo(Poste detail)
        {
            try
            {
                Poste detailTrouve = bdd.Postes.FirstOrDefault(poste => poste.Id == detail.Id);
                if (detailTrouve != null)
                {
                    detailTrouve.Logo = detail.Logo.Trim();                   
                    bdd.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public bool RecupererPoste(int id)
        {
            if (bdd.Postes.Count(u => u.Id == id) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeletePoste(Poste detail)
        {
            try
            {
                var poste = bdd.Postes.Single(o => o.Id == detail.Id);
                bdd.Postes.Remove(poste);

                bdd.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }
       
        public bool MiseAJourFinMissionCollab(Collaborateur collaborateur)
        {
            try
            {
                Collaborateur collabTrouve = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == collaborateur.Id);
                if (collabTrouve != null)
                {
                    collabTrouve.Date_fin_mission = collaborateur.Date_fin_mission;                   
                    collabTrouve.Charge = collaborateur.Charge;
                    bdd.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }

        }
       
        public bool MiseAJourFinContrat(Collaborateur collaborateur)
        {
            try
            {
                Collaborateur collabTrouve = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == collaborateur.Id);
                if (collabTrouve != null)
                {
                    collabTrouve.Date_fin_contrat = collaborateur.Date_fin_contrat;
                    bdd.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool MiseAJourArchive(Collaborateur collaborateur,out Collaborateur detail)
        {
           
            try
            {
                Collaborateur collabTrouve = bdd.Collaborateurs.FirstOrDefault(collab => collab.Id == collaborateur.Id);
                if (collabTrouve != null)
                {
                    collabTrouve.Date_fin_contrat = collaborateur.Date_fin_contrat;
                    collabTrouve.Archive = collaborateur.Archive;
                    bdd.SaveChanges();
                    detail = collabTrouve;
                    return true;
                }
                else
                {
                    detail = null;
                    return false;
                }
            }
            catch
            {
                detail = null;
                return false;
            }
        }
        public string RecupererProfil(int id)
        {
            return bdd.Utilisateurs.Where(u => u.Id.Equals(id)).Select(p => p.Profil).FirstOrDefault();
           
        }
        public bool TestAccessAgence(int idAgence, int idUtil)
        {
            try
            {
                Affect_Utilisateur_Agence agenceTrouve = bdd.Affect_Utilisateur_Agences.FirstOrDefault(collab => collab.fk_id_agence == idAgence && collab.fk_id_utilisateur == idUtil);
                if (agenceTrouve != null)
                {
                   
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
          

        }
        #endregion
        #endregion

        #region Competence

        /// <summary>
        /// Recherche de collaborateur selon leur nom, prénom ou "nom prénom"
        /// </summary>
        /// <param name="query">nom et/ou prénom recherché</param>
        /// <returns>Liste des collaborateurs correspondants</returns>
        public List<string> RechercheCompetences(string query)
        {
            return bdd.Competences.Select(m => m.Nom).Distinct().Where(m =>
              m.ToLower().StartsWith(query.ToLower())
               ).ToList();
        }

        /// <summary>
        /// Recherche de collaborateur selon leurs compétences
        /// </summary>
        /// <param name="query">recherche de l'utilisateur</param>
        /// <param name="dispo">disponibilité, 1 : disponible, 2 : dans 3 mois ou moins, 3 : indisponible</param>
        /// <param name="other_agency">id d'une autre agence pour les directeurs d'agence</param>
        /// <returns>Liste des collaborateurs avec leurs compétences</returns>
        public List<CollabCompet> RechercheCollaborateurWithCompet(string[] query, int[] dispo, int other_agency)
        {
            List<CollabCompet> collabCompets = new List<CollabCompet>();
            var queryJoin = bdd.Competences.Join(bdd.Collaborateurs,
                comp => comp.IdCollab,
                collab => collab.Id,
                (comp, collab) => new
                {
                    Comp = comp,
                    Collab = collab
                });

            // tri selon la disponibilité
            DateTime today3month = DateTime.Now.AddMonths(3);
            if (dispo != null && dispo.Contains(1))
                queryJoin = queryJoin.Where(m => m.Collab.Date_fin_mission == null || m.Collab.Date_fin_mission.Value <= DateTime.Now);
            if (dispo != null && dispo.Contains(2))
                queryJoin = queryJoin.Where(m => today3month >= m.Collab.Date_fin_mission.Value);
            if (dispo != null && dispo.Contains(3))
                queryJoin = queryJoin.Where(m => today3month < m.Collab.Date_fin_mission.Value);

            //TODO : gestion des agences (agence de l'utilisateur connecté + autres agences éventuelles)

            string comp1 = query[0];
            List<int> collabIds = queryJoin.Where(m => m.Comp.Nom == comp1).OrderByDescending(m => m.Comp.Niveau).Select(m => m.Collab.Id).ToList();
            foreach (int collabId in collabIds)
            {
                CollabCompet collabCompet = new CollabCompet();
                collabCompet.collaborateur = bdd.Collaborateurs.First(m => m.Id == collabId);
                bool compNull = false;

                if (query.Length > 0)
                {
                    collabCompet.comp1 = bdd.Competences.FirstOrDefault(m =>
                      m.IdCollab == collabId && m.Nom == comp1);
                    if (collabCompet.comp1 == null)
                        compNull = true;
                }

                if (query.Length > 1)
                {
                    string comp2 = query[1];
                    collabCompet.comp2 = bdd.Competences.FirstOrDefault(m =>
                      m.IdCollab == collabId && m.Nom == comp2);
                    if (collabCompet.comp2 == null)
                        compNull = true;
                }

                if (query.Length > 2)
                {
                    string comp3 = query[2];
                    collabCompet.comp3 = bdd.Competences.FirstOrDefault(m =>
                      m.IdCollab == collabId && m.Nom == comp3);
                    if (collabCompet.comp3 == null)
                        compNull = true;
                }

                if (!compNull)
                    collabCompets.Add(collabCompet);
            }
            return collabCompets;
        }

        #endregion


        /// <summary>
        /// Ajout d'une compétence
        /// </summary>
        /// /// <param name="idCollab">id du collab qui vient d'être crée</param>
        /// /// <param name="nom">nom spécifique de la compétence (anglais, cobol, gestion de projet...)</param>
        /// /// <param name="type">type de compétence (langue, formation, technique, fonctionelle)</param>
        /// /// <param name="certificat">chemin d'accès complet à la certification</param>
        /// /// <param name="niveau">niveau de la compétence (dépend du type de compétence)</param>
        /// <returns>Null</returns>
        public List<Competence> listComp(int idCollab, string type)
        {
            return bdd.Competences.Where(c => c.IdCollab == idCollab && c.Type == type).ToList();
        }
        public Competence AddSkill(int idCollab, string nom, string type, int niveau)
        {
            Competence comp = new Competence
            {
                IdCollab = idCollab,
                Nom = nom,
                Type = type,
                Niveau = niveau,
            };
            bdd.Competences.Add(comp);
            try
            {
                bdd.SaveChanges();
            }
            catch (Exception ex)
            {
                var Response = ("Error: " + ex.Message);
            }
            return comp;
        }
        public Competence EditCommonSkill(int idCollab, string nom, string type, int niveau)
        {
            Competence comp = bdd.Competences.FirstOrDefault(c => c.IdCollab == idCollab);
            comp.Nom = nom; comp.Niveau = niveau;
            bdd.SaveChanges();
            return comp;
        }
        
        public Competence AddLangSkill(int idCollab, string nom, string type, int niveau, string certificat)
        {
            Competence comp = new Competence
            {
                IdCollab = idCollab,
                Nom = nom,
                Type = type,
                Niveau = niveau,
                Certificat = certificat,
        };
            bdd.Competences.Add(comp);
            try
            {
                bdd.SaveChanges();
            }
            catch (Exception ex)
            {
                var Response = ("Error: " + ex.Message);
            }
            return comp;
        }
        public Competence EditLangSkill(int idCollab,int id, string nom, string type, int niveau, string certificat)
        {
            // Pas oublier qu'il faut MODIFIER les dernieres valeurs et pas juste aouter une nouvelle ligne
            Competence comp = bdd.Competences.FirstOrDefault(c => c.IdCollab == idCollab && c.Id == id);
            comp.Nom = nom; comp.Niveau = niveau; comp.Certificat = certificat;
            bdd.SaveChanges();
            return comp;
        }
        public Etude AddEtude(int idCollab, string intitule, string ville, string domaine, string etablissement, string diplome, DateTime dateDeb, DateTime dateFin)
        {
            Etude formation = new Etude
            {
                IdCollab = idCollab,
                Intitule = intitule,
                Ville = ville,
                Domaine = domaine,
                Etablissement = etablissement,
                Diplome = diplome,
                Date_debut = dateDeb,
                Date_fin = dateFin
            };
            bdd.Etudes.Add(formation);
            try
            {
                bdd.SaveChanges();
            }
            catch (Exception ex)
            {
                
               var Response = ("error");
            }

            return formation;
        }
        public Etude EditFormSkill(int idCollab, string intitule, string ville, string domaine, string etablissement, string diplome, DateTime dateDeb, DateTime dateFin)
        {
            Etude form = bdd.Etudes.FirstOrDefault(c => c.IdCollab == idCollab);
            form.IdCollab = idCollab; form.Intitule = intitule; form.Ville = ville; form.Domaine = domaine;
            form.Etablissement = etablissement; form.Diplome = diplome; form.Date_debut = dateDeb; form.Date_fin = dateFin;
            bdd.SaveChanges();
            return form;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}