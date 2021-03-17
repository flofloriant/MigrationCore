using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apogee.Models
{
    public class ListUrgence
    {
        public ListUrgence(List<Urgence> resultatUrgence)
        {
            ResultatUrgence = resultatUrgence;
        }

        public ListUrgence(int fK_id_collaborateur, List<Urgence> resultatUrgence)
        {
            FK_id_collaborateur = fK_id_collaborateur;
            ResultatUrgence = resultatUrgence;
        }

        public int FK_id_collaborateur { get; set; }
        public List<Urgence> ResultatUrgence { get; set; }
    }
}