using System.Windows;

namespace coffre_fort2.Views
{
    public partial class ChoixFormatImportView : Window
    {
        public string TypeChoisi { get; private set; }

        public ChoixFormatImportView()
        {
            InitializeComponent();
        }

        private void ImporterCsv_Click(object sender, RoutedEventArgs e)
        {
            TypeChoisi = "csv";  // Indiquer que l'utilisateur veut importer en CSV
            DialogResult = true;  // Fermer la fenêtre avec une reponse positive
            Close();
        }

        private void ImporterXml_Click(object sender, RoutedEventArgs e)
        {
            TypeChoisi = "xml";  // Indiquer que l'utilisateur veut importer en XML
            DialogResult = true;
            Close();
        }
    }
}
