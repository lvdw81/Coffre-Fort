using System.Windows;
using coffre_fort2.ViewModels;

namespace coffre_fort2.Views
{
    public partial class ConnexionView : Window
    {
        private readonly ConnexionViewModel _viewModel;

        public ConnexionView()
        {
            InitializeComponent();

            _viewModel = new ConnexionViewModel();
            DataContext = _viewModel;

            ValiderButton.Click += async (s, e) =>
            {
                _viewModel.Identifiant = IdentifiantBox.Text;
                _viewModel.MotDePasse = PasswordBox.Password;

                bool connexionReussie = await _viewModel.ExecuterConnexion();

                if (connexionReussie)
                {
                    MessageBox.Show("Connexion reussie !");
                    var main = new MainView(_viewModel.Identifiant, _viewModel.JwtToken);
                    main.Show();
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
