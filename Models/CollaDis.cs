using System.Collections.Generic;

namespace Apogee.Models
{
    public class CollaDis
    {
        public CollaDis()
        {
        }

        public CollaDis(Collaborateur collaborateur, Photo photo, List<Competence> langues, List<Competence> compTechniques, List<Competence> compFonctionnelles, List<Etude> etudes, List<Formation> formations, List<Urgence> urgences)
        {
            Collaborateur = collaborateur;
            Photo = photo;
            Langues = langues;
            CompTechniques = compTechniques;
            CompFonctionnelles = compFonctionnelles;
            Etudes = etudes;
            Formations = formations;
            Urgences = urgences;
        }

        public CollaDis(Collaborateur collaborateur, Photo photo, List<Competence> langues, List<Competence> compTechniques, List<Competence> compFonctionnelles, List<Etude> etudes, List<Formation> formations, List<Urgence> urgences, Poste posteMax) : this(collaborateur, photo, langues, compTechniques, compFonctionnelles, etudes, formations, urgences)
        {
            PosteMax = posteMax;
        }

        public CollaDis(Collaborateur collaborateur, Photo photo, List<Competence> langues, List<Competence> compTechniques, List<Competence> compFonctionnelles, List<Etude> etudes, List<Formation> formations, List<Urgence> urgences, Poste posteMax, List<Poste> postes) : this(collaborateur, photo, langues, compTechniques, compFonctionnelles, etudes, formations, urgences, posteMax)
        {
            Postes = postes;
        }

        public virtual Collaborateur Collaborateur { get; set; }
        public virtual Photo Photo { get; set; }
        public  List<Competence> Langues { get; set; }
        public List<Competence> CompTechniques { get; set; }
        public List<Competence> CompFonctionnelles { get; set; }
        public virtual List<Etude> Etudes { get; set; }
        public virtual List<Formation> Formations { get; set; }
        public virtual List<Urgence> Urgences { get; set; }
        public virtual Poste PosteMax { get; set; }
        public virtual List<Poste> Postes { get; set; }


    }
}