using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using coffre_fort2.Models;
using coffre_fort2.Services;
using coffre_fort2.Views;

namespace coffre_fort2.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly string _utilisateur;
        private readonly PasswordService _passwordService;
        private int _utilisateurId;
        private readonly string _jwtToken;


        public ObservableCollection<PasswordEntry> Entrees { get; set; } = new();

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        public ICommand OuvrirFenetrePartageCommande { get; }
        public ICommand ViewSharedPasswordsCommand { get; }
        public ICommand ImporterCommande { get; }
        public ICommand ExporterCommande { get; }
        public ICommand CopierIdentifiantCommande { get; }
        public ICommand CopierMotDePasseCommande { get; }
        public ICommand AjouterCommande { get; }
        public ICommand SupprimerCommande { get; }
        public ICommand ModifierCommande { get; }
        public ICommand ToggleMotDePasseCommande { get; }

        private PasswordEntry _entreeSelectionnee = new();
        public PasswordEntry EntreeSelectionnee
        {
            get => _entreeSelectionnee;
            set => SetProperty(ref _entreeSelectionnee, value);
        }

        public MainViewModel(string utilisateur, string jwtToken)
        {
            _utilisateur = utilisateur;
            _passwordService = new PasswordService(jwtToken);
            _jwtToken = jwtToken;
            OuvrirFenetrePartageCommande = new RelayCommandAsync(async param => await OuvrirFenetrePartageAsync(param));
            ViewSharedPasswordsCommand = new RelayCommand(async _ => await OuvrirVuePartage());
            ImporterCommande = new RelayCommand(_ => OuvrirFenetreImport());
            ExporterCommande = new RelayCommand(_ => ChoisirTypeExport());
            AjouterCommande = new RelayCommandAsync(async _ => await OuvrirFenetreAjoutAsync());
            SupprimerCommande = new RelayCommandAsync(async _ => await SupprimerEntreeAsync());
            ModifierCommande = new RelayCommandAsync(async _ => await ModifierEntreeAsync());
            ToggleMotDePasseCommande = new RelayCommand(obj => ToggleMotDePasse(obj));
            CopierIdentifiantCommande = new RelayCommand(obj => CopierIdentifiant(obj));
            CopierMotDePasseCommande = new RelayCommand(obj => CopierMotDePasse(obj));

            _ = ChargerDepuisApiAsync();
        }

        private async Task ChargerDepuisApiAsync()
        {
            try
            {
                var userService = new UserService(_passwordService.JwtToken);
                var user = await userService.GetUserByIdentifiantAsync(_utilisateur);
                if (user == null)
                {
                    MessageBox.Show("Utilisateur introuvable.");
                    return;
                }
                _utilisateurId = user.Id;

                var motsDePasse = await _passwordService.GetByUserIdAsync(user.Id);
                Entrees.Clear();
                foreach (var entry in motsDePasse)
                    Entrees.Add(entry);

                Message = $"{Entrees.Count} mot(s) de passe charge(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des donnees : " + ex.Message);
            }
        }


        private async Task OuvrirFenetreAjoutAsync()
        {
            var fenetre = new EntreeFormView(_utilisateur);
            var result = fenetre.ShowDialog();

            if (result == true && fenetre.Entree != null)
            {
                try
                {
                    fenetre.Entree.UtilisateurId = _utilisateurId;
                    var ajoutee = await _passwordService.CreateAsync(fenetre.Entree);
                    Entrees.Add(ajoutee);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur pendant l'ajout : " + ex.Message);
                }
            }
        }

        private async Task ModifierEntreeAsync()
        {
            if (EntreeSelectionnee == null)
                return;

            var fenetre = new EntreeFormView(_utilisateur, EntreeSelectionnee);
            var result = fenetre.ShowDialog();

            if (result == true && fenetre.Entree != null)
            {
                try
                {
                    fenetre.Entree.UtilisateurId = _utilisateurId;
                    var updated = await _passwordService.UpdateAsync(fenetre.Entree);

                    EntreeSelectionnee.NomApplication = updated.NomApplication;
                    EntreeSelectionnee.Identifiant = updated.Identifiant;
                    EntreeSelectionnee.MotDePasse = updated.MotDePasse;
                    EntreeSelectionnee.Tags = new List<string>(updated.Tags);

                    OnPropertyChanged(nameof(EntreeSelectionnee));
                    OnPropertyChanged(nameof(Entrees));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur pendant la modification : " + ex.Message);
                }
            }
        }

        private async Task SupprimerEntreeAsync()
        {
            if (EntreeSelectionnee == null)
                return;

            try
            {
                bool success = await _passwordService.DeleteAsync(EntreeSelectionnee.Id);
                if (success)
                {
                    Entrees.Remove(EntreeSelectionnee);
                }
                else
                {
                    MessageBox.Show("Echec de la suppression cote serveur.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur pendant la suppression : " + ex.Message);
            }
        }

        private void ToggleMotDePasse(object obj)
        {
            if (obj is PasswordEntry entry)
            {
                entry.AfficherMotDePasse = !entry.AfficherMotDePasse;
                OnPropertyChanged(nameof(Entrees));
            }
        }

        private void CopierIdentifiant(object obj)
        {
            if (obj is PasswordEntry entry)
            {
                Clipboard.SetText(entry.Identifiant);
                MessageBox.Show($"Identifiant copie : {entry.Identifiant}", "Copie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CopierMotDePasse(object obj)
        {
            if (obj is PasswordEntry entry)
            {
                Clipboard.SetText(entry.MotDePasse);
                MessageBox.Show("Mot de passe copie dans le presse-papiers.", "Copie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExporterEnCsv()
        {
            try
            {
                string chemin = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                using var writer = new StreamWriter(chemin);
                writer.WriteLine("Nom;Identifiant;Mot de passe;Tags");

                foreach (var entry in Entrees)
                {
                    string tags = string.Join(",", entry.Tags ?? new List<string>());
                    writer.WriteLine($"{entry.NomApplication};{entry.Identifiant};{entry.MotDePasse};{tags}");
                }

                MessageBox.Show($"Export termine :\n{chemin}", "Export CSV", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur export CSV : " + ex.Message);
            }
        }



        private void ExporterEnXml()
        {
            try
            {
                string chemin = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PasswordEntry>));
                using var writer = new StreamWriter(chemin);
                serializer.Serialize(writer, new List<PasswordEntry>(Entrees));
                MessageBox.Show($"Export XML termine :\n{chemin}", "Export XML", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur export XML : " + ex.Message);
            }
        }

        private void ChoisirTypeExport()
        {
            var choixFenetre = new ChoixFormatView();
            var result = choixFenetre.ShowDialog();

            if (result == true)
            {
                if (choixFenetre.TypeChoisi == "csv")
                    ExporterEnCsv();
                else if (choixFenetre.TypeChoisi == "xml")
                    ExporterEnXml();
            }
        }



        private async void ImporterDepuisCsv()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Fichier CSV (*.csv)|*.csv",
                Title = "Importer des mots de passe"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var nouvellesEntrees = new List<PasswordEntry>();
                    using (var reader = new StreamReader(openFileDialog.FileName))
                    {
                        bool firstLine = true;
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (firstLine)
                            {
                                firstLine = false; // Sauter l'entête si necessaire
                                continue;
                            }

                            var values = line.Split(';'); // On suppose que les colonnes sont separees par des points-virgules
                            if (values.Length >= 4) // Verifier qu'il y a suffisamment de colonnes
                            {
                                var entry = new PasswordEntry
                                {
                                    NomApplication = values[0], // NomApplication
                                    Identifiant = values[1], // Identifiant
                                    MotDePasse = values[2], // MotDePasse
                                    Tags = new List<string> { values[3] }, // Tags, peut-être a adapter selon ton format CSV
                                    Utilisateur = _utilisateur,
                                    UtilisateurId = _utilisateurId
                                };

                                nouvellesEntrees.Add(entry);
                            }
                        }
                    }

                    foreach (var entry in nouvellesEntrees)
                    {
                        var ajoutee = await _passwordService.CreateAsync(entry);
                        Entrees.Add(ajoutee);
                    }

                    MessageBox.Show("Import et sauvegarde termine.", "Import CSV", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur import CSV : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private async void ImporterDepuisXml()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Fichier XML (*.xml)|*.xml",
                Title = "Importer des mots de passe"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PasswordEntry>));
                    using var reader = new StreamReader(openFileDialog.FileName);
                    var nouvellesEntrees = (List<PasswordEntry>)serializer.Deserialize(reader);

                    foreach (var entry in nouvellesEntrees)
                    {
                        entry.Utilisateur = _utilisateur;
                        entry.UtilisateurId = _utilisateurId;

                        var ajoutee = await _passwordService.CreateAsync(entry);
                        Entrees.Add(ajoutee);
                    }

                    MessageBox.Show("Import et sauvegarde termine.", "Import XML", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur import XML : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OuvrirFenetreImport()
        {
            var fenetreImport = new ChoixFormatImportView();
            var result = fenetreImport.ShowDialog();

            // Verifier que l'utilisateur a fait un choix
            if (result == true)
            {
                // Recuperer le type de format choisi (csv ou xml)
                string typeChoisi = fenetreImport.TypeChoisi;

                if (typeChoisi == "csv")
                {
                    // Appeler la fonction d'importation CSV
                    ImporterDepuisCsv();
                }
                else if (typeChoisi == "xml")
                {
                    // Appeler la fonction d'importation XML
                    ImporterDepuisXml();
                }
            }
            else
            {
                // Message d'erreur si aucun format n'est choisi
                MessageBox.Show("Aucun format choisi", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private async Task OuvrirVuePartage()
        {
            var viewModel = new VoirMotsDePassePartagesViewModel(_utilisateurId, _passwordService);
            var view = new VoirMotsDePassePartagesView { DataContext = viewModel };
            view.ShowDialog();
        }


        private async Task OuvrirFenetrePartageAsync(object param)
        {
            if (param is not PasswordEntry entree)
                return;

            var fenetre = new PartagerMotDePasseView();
            fenetre.DataContext = new PartagerMotDePasseViewModel(_passwordService, _utilisateurId, () => fenetre.Close(), _jwtToken);
            fenetre.ShowDialog();
        }




    }
}
