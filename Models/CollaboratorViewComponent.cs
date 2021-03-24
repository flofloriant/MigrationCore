using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apogee.Models
{
    public class CollaboratorViewComponent : ViewComponent
    {
        private IDal dal;

        public IViewComponentResult Invoke()
        {
            #region Importer Exporter une photo
            #region PhotoVue Importer en mode Get
            /// <summary>
            /// Vue photo pour importer ou exporter photo
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            /*Collaborateur collaborateur = dal.RecupererCollaborateur(9);*/
            Collaborateur collaborateur = new Collaborateur();
            Photo photo = new Photo();
                photo.Erreur = string.Empty;
                photo.Fic_photo_path = collaborateur.Fic_photo;
                photo.Id = collaborateur.Id;
                photo.Fic_autorisation_photoCollab = collaborateur.Fic_autorisation_photo;
                photo.Fic_photoCollab = collaborateur.Fic_photo;
                return View("PhotoVue", photo);
            #endregion
            #endregion
        }
    }
}
