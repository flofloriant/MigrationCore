using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apogee.Models
{
    public class PhotoVue
    {
        public PhotoVue(Collaborateur collaborateurDetail, Photo photoDetail)
        {
            CollaborateurDetail = collaborateurDetail;
            PhotoDetail = photoDetail;
        }

        public Collaborateur CollaborateurDetail { get; set; }
        public Photo PhotoDetail { get; set; }
    }
}