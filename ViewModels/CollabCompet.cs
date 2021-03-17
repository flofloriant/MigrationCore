using Apogee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apogee.ViewModels
{
    public class CollabCompet
    {
        public Collaborateur collaborateur { get; set; }
        public Competence comp1 { get; set; }
        public Competence comp2 { get; set; }
        public Competence comp3 { get; set; }
    }
}