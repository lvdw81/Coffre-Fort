using System.Windows.Input;
using coffre_fort2.Models;

namespace coffre_fort2.ViewModels
{
    public class ConnexionViewModel : BaseViewModel
    {
        private string _identifiant = "";
        public string Identifiant
        {
            get => _identifiant;
            set => SetProperty(ref _identifiant, value);
        }

        private string _motDePasse = "";
        public string MotDePasse
        {
            get => _motDePasse;
            set => SetProperty(ref _motDePasse, value);
        }

        private string _message = "";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand ValiderCommande { get; }

        public ConnexionViewModel()
        {
            ValiderCommande = new RelayCommand(ExecuterConnexion);
        }

        private void ExecuterConnexion(object obj)
        {
            if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(MotDePasse))
            {
                Message = "Champs obligatoires";
                return;
            }

            bool ok = UserManager.VerifierConnexion(Identifiant, MotDePasse);
            Message = ok ? "Connexion reussie" : "Identifiants incorrects";
        }
    }
}
