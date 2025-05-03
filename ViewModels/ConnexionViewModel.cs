using System.Threading.Tasks;
using System.Windows.Input;
using coffre_fort2.Services;

namespace coffre_fort2.ViewModels
{
    public class ConnexionViewModel : BaseViewModel
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

        public ICommand ValiderCommande { get; }

        public string JwtToken => _authService.JwtToken;

        public ConnexionViewModel()
        {
            _authService = new AuthService();
            ValiderCommande = new RelayCommandAsync(async _ => await ExecuterConnexion());
        }

        public async Task<bool> ExecuterConnexion()
        {
            if (string.IsNullOrWhiteSpace(Identifiant) || string.IsNullOrWhiteSpace(MotDePasse))
            {
                Message = "Champs obligatoires";
                return false;
            }

            bool ok = await _authService.LoginAsync(Identifiant, MotDePasse);
            Message = ok ? "Connexion reussie" : "Identifiants incorrects";
            return ok;
        }
    }
}
