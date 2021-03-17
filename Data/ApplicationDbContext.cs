using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Apogee.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apogee.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Collaborateur> Collaborateurs { get; set; }
        public DbSet<Agence> Agences { get; set; }
        public DbSet<Affect_Utilisateur_Agence> Affect_Utilisateur_Agences { get; set; }
        public DbSet<Coefficient> Coefficients { get; set; }
        public DbSet<Contrat> Contrats { get; set; }
        public DbSet<Niveau> Niveaux { get; set; }
        public DbSet<Statut_Contrat> Statut_Contrats { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<Etude> Etudes { get; set; }
        public DbSet<Formation> Formations { get; set; }
        public DbSet<Urgence> Urgences { get; set; }
        public DbSet<Poste> Postes { get; set; }
    }
}
