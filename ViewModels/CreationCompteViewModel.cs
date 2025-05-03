using System.Threading.Tasks;
using System.Windows.Input;
using coffre_fort2.Services;

namespace coffre_fort2.ViewModels
{
    public class CreationCompteViewModel : BaseViewModel
    {
        private readonly AuthService _authService;

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
            _authService = new AuthService();
            CreerCommande = new RelayCommandAsync(async _ => await ExecuterCreation());
        }

        public async Task ExecuterCreation()
        {
            if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(MotDePasse))
            {
                Message = "Champs obligatoires";
                return;
            }

            var resultat = await _authService.RegisterAsync(Identifiant, MotDePasse);
            Message = resultat == "ok"
                ? "Compte cree avec succes"
                : resultat;
        }
    }
}
