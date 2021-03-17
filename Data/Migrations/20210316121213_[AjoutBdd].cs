using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Apogee.Data.Migrations
{
    public partial class AjoutBdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Affect_Utilisateur_Agence",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fk_id_utilisateur = table.Column<int>(nullable: false),
                    fk_id_agence = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Affect_Utilisateur_Agence", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Agence",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code_pays = table.Column<string>(nullable: false),
                    nom = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agence", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Coefficient",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    valeur = table.Column<string>(nullable: false),
                    FK_id_niveau = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coefficient", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Collaborateur",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    matricule = table.Column<int>(nullable: false),
                    civilite = table.Column<int>(nullable: false),
                    nom = table.Column<string>(maxLength: 30, nullable: false),
                    prenom = table.Column<string>(maxLength: 30, nullable: false),
                    fic_photo = table.Column<string>(nullable: true),
                    fic_autorisation_photo = table.Column<string>(nullable: true),
                    FK_id_agence = table.Column<int>(nullable: false),
                    adresse = table.Column<string>(maxLength: 50, nullable: false),
                    code_postal = table.Column<string>(maxLength: 10, nullable: false),
                    ville = table.Column<string>(maxLength: 50, nullable: false),
                    pays = table.Column<string>(maxLength: 30, nullable: false),
                    date_naissance = table.Column<DateTime>(nullable: false),
                    email = table.Column<string>(maxLength: 50, nullable: false),
                    email_travail = table.Column<string>(maxLength: 50, nullable: false),
                    tel = table.Column<string>(maxLength: 20, nullable: false),
                    tel_travail = table.Column<string>(maxLength: 20, nullable: false),
                    FK_id_statut = table.Column<int>(nullable: false),
                    FK_id_contrat = table.Column<int>(nullable: false),
                    FK_id_niveau = table.Column<int>(nullable: false),
                    FK_id_coefficient = table.Column<int>(nullable: false),
                    salaire_brut_mens = table.Column<double>(nullable: false),
                    date_fin_mission = table.Column<DateTime>(nullable: true),
                    date_embauche = table.Column<DateTime>(nullable: false),
                    date_fin_contrat = table.Column<DateTime>(nullable: true),
                    archive = table.Column<bool>(nullable: false),
                    salaire_bru_ann = table.Column<double>(nullable: false),
                    charge = table.Column<int>(nullable: false),
                    suffixe_homonyme = table.Column<int>(nullable: false),
                    abilite_defense = table.Column<bool>(nullable: false),
                    FK_id_commercial = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collaborateur", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Contrat",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code_pays = table.Column<string>(nullable: false),
                    nom = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contrat", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Etude",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_id_collaborateur = table.Column<int>(nullable: false),
                    intitule = table.Column<string>(maxLength: 30, nullable: false),
                    etablissement = table.Column<string>(maxLength: 30, nullable: false),
                    date_debut = table.Column<DateTime>(nullable: false),
                    date_fin = table.Column<DateTime>(nullable: false),
                    ville = table.Column<string>(nullable: true),
                    domaine = table.Column<string>(nullable: true),
                    fic_diplome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etude", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Formation",
                columns: table => new
                {
                    num_demande = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_id_collaborateur = table.Column<int>(nullable: false),
                    intitule = table.Column<string>(nullable: true),
                    certification = table.Column<bool>(nullable: false),
                    date_demande = table.Column<DateTime>(nullable: false),
                    date_acceptation = table.Column<DateTime>(nullable: false),
                    FK_id_statut = table.Column<int>(nullable: false),
                    date_debut = table.Column<DateTime>(nullable: false),
                    date_fin = table.Column<DateTime>(nullable: false),
                    validation = table.Column<bool>(nullable: false),
                    organisme = table.Column<string>(nullable: true),
                    fic_certification = table.Column<string>(nullable: true),
                    fic_devis = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formation", x => x.num_demande);
                });

            migrationBuilder.CreateTable(
                name: "Niveau",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code_pays = table.Column<string>(nullable: false),
                    valeur = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Niveau", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Poste",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_id_collaborateur = table.Column<int>(nullable: false),
                    nom_client = table.Column<string>(maxLength: 50, nullable: false),
                    description = table.Column<string>(nullable: true),
                    date_debut = table.Column<DateTime>(nullable: false),
                    date_fin = table.Column<DateTime>(nullable: false),
                    intitule = table.Column<string>(maxLength: 100, nullable: false),
                    ville = table.Column<string>(maxLength: 50, nullable: false),
                    pays = table.Column<string>(maxLength: 50, nullable: false),
                    charge = table.Column<int>(nullable: false),
                    logo = table.Column<string>(nullable: true),
                    domaine = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poste", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Statut_Contrat",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code_pays = table.Column<string>(nullable: false),
                    nom = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statut_Contrat", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Urgence",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_id_collaborateur = table.Column<int>(nullable: false),
                    nom = table.Column<string>(maxLength: 30, nullable: false),
                    prenom = table.Column<string>(maxLength: 30, nullable: false),
                    adresse = table.Column<string>(maxLength: 150, nullable: false),
                    tel_fixe = table.Column<string>(maxLength: 20, nullable: false),
                    tel_port = table.Column<string>(maxLength: 20, nullable: false),
                    tel_travail = table.Column<string>(maxLength: 20, nullable: true),
                    lien = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urgence", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nom = table.Column<string>(nullable: true),
                    prenom = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: false),
                    mot_de_passe = table.Column<string>(nullable: false),
                    profil = table.Column<string>(nullable: true),
                    compteur_mdp_faux = table.Column<int>(nullable: false),
                    validite_mdp = table.Column<DateTime>(nullable: true),
                    date_bloquage = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateur", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Competence",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_id_collaborateur = table.Column<int>(nullable: false),
                    nom = table.Column<string>(maxLength: 30, nullable: true),
                    type = table.Column<string>(maxLength: 1, nullable: true),
                    niveau = table.Column<int>(nullable: false),
                    fic_certificat = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competence", x => x.id);
                    table.ForeignKey(
                        name: "FK_Competence_Collaborateur_FK_id_collaborateur",
                        column: x => x.FK_id_collaborateur,
                        principalTable: "Collaborateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Competence_FK_id_collaborateur",
                table: "Competence",
                column: "FK_id_collaborateur");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Affect_Utilisateur_Agence");

            migrationBuilder.DropTable(
                name: "Agence");

            migrationBuilder.DropTable(
                name: "Coefficient");

            migrationBuilder.DropTable(
                name: "Competence");

            migrationBuilder.DropTable(
                name: "Contrat");

            migrationBuilder.DropTable(
                name: "Etude");

            migrationBuilder.DropTable(
                name: "Formation");

            migrationBuilder.DropTable(
                name: "Niveau");

            migrationBuilder.DropTable(
                name: "Poste");

            migrationBuilder.DropTable(
                name: "Statut_Contrat");

            migrationBuilder.DropTable(
                name: "Urgence");

            migrationBuilder.DropTable(
                name: "Utilisateur");

            migrationBuilder.DropTable(
                name: "Collaborateur");
        }
    }
}
