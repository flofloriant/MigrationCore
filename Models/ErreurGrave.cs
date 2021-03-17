using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apogee.Models
{
    public class ErreurGrave
    {
        public string MessageTitre { get;  set; }
        public string MessageErreur { get;  set; }
        public string CommentaireErreur { get; set; }
        public ErreurGrave(string messageTitre, string messageErreur, string commentaireErreur)
        {
            MessageTitre = messageTitre;
            MessageErreur = messageErreur;
            CommentaireErreur = commentaireErreur;
        }
    }
}