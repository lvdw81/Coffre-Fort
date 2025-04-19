using System.Windows;
using coffre_fort2.ViewModels;

namespace coffre_fort2.Views
{
    public partial class ConnexionView : Window
    {
        private ConnexionViewModel _viewModel;

        public ConnexionView()
        {
            InitializeComponent();

            _viewModel = new ConnexionViewModel();
            DataContext = _viewModel;

            ValiderButton.Click += (s, e) =>
            {
                _viewModel.Identifiant = IdentifiantBox.Text;
                _viewModel.MotDePasse = PasswordBox.Password;

                if (_viewModel.ValiderCommande.CanExecute(null))
                    _viewModel.ValiderCommande.Execute(null);

                if (_viewModel.Message == "Connexion reussie")
                {
                    MessageBox.Show("Connexion reussie !");
                    // Tu peux ici ouvrir la fenetre principale plus tard
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
