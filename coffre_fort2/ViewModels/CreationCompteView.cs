using System.Windows.Input;
using coffre_fort2.Models;

namespace coffre_fort2.ViewModels
{
    public class CreationCompteViewModel : BaseViewModel
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


        public ICommand CreerCommande { get; }

        public CreationCompteViewModel()
        {
            CreerCommande = new RelayCommand(ExecuterCreation);
        }

        private void ExecuterCreation(object obj)
        {
            if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(MotDePasse))
            {
                Message = "Champs obligatoires";
                return;
            }

            bool creationReussie = UserManager.AjouterUtilisateur(Identifiant, MotDePasse);
            Message = creationReussie ? "Compte cree avec succes" : "Identifiant deja utilise";
        }
    }
}
