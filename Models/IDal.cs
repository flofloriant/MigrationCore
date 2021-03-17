using Apogee.ViewModels;
using System;
using System.Collections.Generic;

namespace Apogee.Models
{
    public interface IDal : IDisposable
    {
        Utilisateur RechercheUtilisateur(string email);    
        Collaborateur RecupererCollaborateur(int id);
        List<Collaborateur> RechercheCollaborateur(string query);
        List<Collaborateur> RechercheCollaborateur(string query, int[] dispo, int ordre, int charge, int other_agence = 0);
        LoginReturnViewModel Authentifier(string email, string motDePasse);
        Utilisateur RecupererUtilisateur(int? id);
        List<Agence> ObtientTousLesAgences(int id);
        int ObtenirFirstAgence(int id);
        Agence ObtenirAgence(int id);     
        List<Affect_Utilisateur_Agence> ObtientTousLesIdAgences(int id);
        public int ObtenirPaysAgence(List<Agence> detail);
        List<Contrat> ObtientTousLesContrats(string pays);
        int ObtenirFirstContrat(string pays);
        int ObtenirFirstStatut(string pays);
        int ObtenirFirstNiveau(string pays);
     
        int ObtenirFirstCoefficient(int fK_id_niveau);
        int ObtientFirstCommercial(int id, string comm);
        int DernierIdCollab(Collaborateur collaborateur);
        bool ContratExiste(int id, string code_pays);
        List<Niveau> ObtientTousLesNiveaux(string pays);
        bool NiveauExiste(int id, string code_pays);
        List<Statut_Contrat> ObtientTousLesStatutContrat(string pays);
        bool StatutExiste(int id, string code_pays);
        List<Coefficient> ObtientTousLesCoefficients(int id);
        bool CoefficientExiste(int id, int fK_id_niveau);
        bool CommercialExiste(int fK_id_agence, int fK_id_commercial);
        List<Utilisateur> ObtientTousLesCommercials(int id, string comm);
        bool MatriculeExiste(int matricule, int id);     
        bool NomPrenomExiste(string nom, string prenom);
        int NomPrenomMax(string nom, string prenom);
        string PaysAgence(int id);
        bool CreerCollaborateur(Collaborateur collaborateur);
        bool MiseAJourCollab(Collaborateur collaborateur);
        List<Competence> RecupererCompetences(int id, string comp);
        List<Etude> RecupererEtudes(int id);
        List<Formation> RecupererFormations(int id);
        List<Urgence> RecupererUrgences(int id);
        Collaborateur ObtientPhotoDetail(int id);
        bool MiseAJourPhoto(Collaborateur collaborateur);
        Urgence CreerUrgence(Urgence detail);
        string ChangePassword(int id, string motDePasse);
        Competence AddSkill(int idCollab, string nom, string type, int niveau);
        Competence AddLangSkill(int idCollab, string nom, string type, int niveau, string certificat);
        Etude AddEtude(int idCollab, string intitule, string ville, string domaine, string etablissement, string diplome, DateTime dateDeb, DateTime dateFin);
        Competence EditLangSkill(int idCollab, int id, string nom, string type, int niveau, string certificat);
        Competence EditCommonSkill(int idCollab, string nom, string type, int niveau);
        Etude EditFormSkill(int idCollab, string intitule, string ville, string domaine, string etablissement, string diplome, DateTime dateDeb, DateTime dateFin);
        List<Competence> listComp(int idCollab, string type);
        string ResetPassword(string email);
        void ResetValiditeMdp(Utilisateur utilisateur);
        List<string> RechercheCompetences(string query);
        List<CollabCompet> RechercheCollaborateurWithCompet(string[] query, int[] dispo, int other_agency);
        bool MiseAJourUrgence(Urgence detail);
        bool RecupererUrgence(int id);
        bool DeleteUrgence(Urgence detail);
        public bool PhotoExiste(int id);    
        bool MiseAJourFinContrat(Collaborateur collaborateur);       
        public bool MiseAJourArchive(Collaborateur collaborateur, out Collaborateur detail);
        List<Poste> RecupererExperiences(int id);
        Poste CreerPoste(Poste detail);
        int DernierIdPoste();
        Poste DateFinMax(int id);
        public Poste ObtientPosteDetail(int id);
        public bool MiseAJourPoste(Poste detail);
        public bool MiseAJourLogo(Poste detail);
        bool RecupererPoste(int id);
        public bool DeletePoste(Poste detail);
        bool MiseAJourFinMissionCollab(Collaborateur collaborateur);
        public string RecupererProfil(int id);
        public bool TestAccessAgence(int idAgence, int idUtil);
    }
}