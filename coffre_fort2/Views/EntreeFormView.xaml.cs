using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using coffre_fort2.Models;

namespace coffre_fort2.Views
{
    public partial class EntreeFormView : Window, INotifyPropertyChanged
    {
        public PasswordEntry Entree { get; private set; }

        private string _tagsTexte = "";
        public string TagsTexte
        {
            get => _tagsTexte;
            set
            {
                _tagsTexte = value;
                OnPropertyChanged(nameof(TagsTexte));
            }
        }

        private bool _tagsActifs;
        public bool TagsActifs
        {
            get => _tagsActifs;
            set
            {
                _tagsActifs = value;
                OnPropertyChanged(nameof(TagsActifs));
            }
        }

        public EntreeFormView(string utilisateur, PasswordEntry entreeExistante = null)
        {
            InitializeComponent();

            if (entreeExistante != null)
            {
                Entree = new PasswordEntry
                {
                    Id = entreeExistante.Id,
                    NomApplication = entreeExistante.NomApplication,
                    Identifiant = entreeExistante.Identifiant,
                    MotDePasse = entreeExistante.MotDePasse,
                    Utilisateur = entreeExistante.Utilisateur,
                    Tags = new List<string>(entreeExistante.Tags)
                };

                TagsTexte = string.Join(", ", Entree.Tags);
                TagsActifs = Entree.Tags.Any();
                Title = "Modifier une entree";
            }
            else
            {
                Entree = new PasswordEntry { Utilisateur = utilisateur };
                TagsTexte = "";
                TagsActifs = false;
                Title = "Nouvelle entree";
            }

            DataContext = this;
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            Entree.NomApplication = NomBox.Text;
            Entree.Identifiant = IdentifiantBox.Text;
            Entree.MotDePasse = MotDePasseBox.Text;


            if (string.IsNullOrWhiteSpace(Entree.NomApplication) ||
                string.IsNullOrWhiteSpace(Entree.Identifiant) ||
                string.IsNullOrWhiteSpace(Entree.MotDePasse))
            {
                MessageBox.Show("Les champs Nom, Identifiant et Mot de passe sont obligatoires.");
                return;
            }
            


            Entree.Tags = TagsActifs
                ? TagsTexte.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(t => t.Trim())
                           .Where(t => !string.IsNullOrWhiteSpace(t))
                           .ToList()
                : new List<string>();

            DialogResult = true;
            Close();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string nom)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));
        }
    }
}
