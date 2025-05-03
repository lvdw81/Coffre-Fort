using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using coffre_fort2.Models;
using coffre_fort2.Services;

namespace coffre_fort2.ViewModels
{
    public class VoirMotsDePassePartagesViewModel : BaseViewModel
    {
        private readonly PasswordService _passwordService;

        public ObservableCollection<PasswordEntry> MotsDePassePartages { get; set; } = new();

        public VoirMotsDePassePartagesViewModel(int userId, PasswordService passwordService)
        {
            _passwordService = passwordService;
            _ = ChargerAsync(userId);
        }

        private async Task ChargerAsync(int userId)
        {
            try
            {
                var resultats = await _passwordService.GetAllSharedPasswordsAsync(userId);

                if (resultats.Count == 0)
                {
                    MessageBox.Show("Aucun mot de passe partagé trouvé.");
                }

                MotsDePassePartages.Clear();
                foreach (var entry in resultats)
                {
                    MotsDePassePartages.Add(entry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement partages : " + ex.Message);
            }
        }

    }
}
