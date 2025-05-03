using System.Windows;

namespace coffre_fort2.Views
{
    public partial class ChoixFormatView : Window
    {
        public string TypeChoisi { get; private set; }

        public ChoixFormatView()
        {
            InitializeComponent();
        }

        // Methode pour gerer l'evenement "Exporter en CSV"
        private void ExporterCsv_Click(object sender, RoutedEventArgs e)
        {
            TypeChoisi = "csv";  // On stocke le format choisi
            DialogResult = true;  // On ferme la fenêtre
            Close();  // On ferme la fenêtre de choix
        }

        // Methode pour gerer l'evenement "Exporter en XML"
        private void ExporterXml_Click(object sender, RoutedEventArgs e)
        {
            TypeChoisi = "xml";  // On stocke le format choisi
            DialogResult = true;  // On ferme la fenêtre
            Close();  // On ferme la fenêtre de choix
        }
    }
}
