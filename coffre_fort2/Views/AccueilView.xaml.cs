using System.Windows;

namespace coffre_fort2.Views
{
    public partial class AccueilView : Window
    {
        public AccueilView()
        {
            InitializeComponent();

            ConnexionButton.Click += (s, e) =>
            {
                var fenetreConnexion = new ConnexionView();
                fenetreConnexion.Show();
                this.Close();
            };

            CreationButton.Click += (s, e) =>
            {
                var fenetreCreation = new CreationCompteView();
                fenetreCreation.Show();
                this.Close();
            };
        }
    }
}
