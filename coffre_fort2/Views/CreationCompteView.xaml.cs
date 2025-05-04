using System.Text.RegularExpressions;
using System.Windows;
using coffre_fort2.ViewModels;

namespace coffre_fort2.Views
{
    public partial class CreationCompteView : Window
    {
        private readonly CreationCompteViewModel _viewModel;

        public CreationCompteView()
        {
            InitializeComponent();
            _viewModel = new CreationCompteViewModel();
            DataContext = _viewModel;

            CreerButton.Click += async (s, e) =>
            {
                _viewModel.Identifiant = IdentifiantBox.Text;
                _viewModel.MotDePasse = PasswordBox.Password;

                // Vérifie que l'identifiant est une adresse Gmail valide
                if (!Regex.IsMatch(_viewModel.Identifiant, @"^[^@\s]+@gmail\.com$", RegexOptions.IgnoreCase))
                {
                    MessageBox.Show("L'identifiant doit être une adresse @gmail.com.");
                    return;
                }

                await _viewModel.ExecuterCreation();

                if (_viewModel.Message == "Compte cree avec succes")
                {
                    MessageBox.Show("Compte cree avec succes !");
                    var retourAccueil = new AccueilView();
                    retourAccueil.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(_viewModel.Message);
                }
            };
        



        RetourButton.Click += (s, e) =>
            {
                var retourAccueil = new AccueilView();
                retourAccueil.Show();
                this.Close();
            };
        }
    }
}
