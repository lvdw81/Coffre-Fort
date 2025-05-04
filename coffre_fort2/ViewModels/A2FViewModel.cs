using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using coffre_fort2.Views;

namespace coffre_fort2.ViewModels
{
    public class A2FViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient = new();

        private string _code = "";
        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        private string _message = "";
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public ICommand VerifierCodeCommande { get; }

        public A2FViewModel()
        {
            VerifierCodeCommande = new RelayCommandAsync(async _ => await VerifierCode());
        }

        private async Task VerifierCode()
        {
            if (string.IsNullOrWhiteSpace(Code))
            {
                Message = "Veuillez entrer le code.";
                return;
            }

            if (!Application.Current.Properties.Contains("email_temp"))
            {
                Message = "Email non trouvé en mémoire.";
                return;
            }

            var email = App.Current.Properties["email_temp"] as string;
            var donnees = new { Identifiant = email, Code = Code };
            var json = JsonSerializer.Serialize(donnees);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://localhost:7182/api/auth/verifier-code", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var doc = JsonDocument.Parse(responseBody);
                    var token = doc.RootElement.GetProperty("token").GetString();
                    var userId = doc.RootElement.GetProperty("userId").GetInt32();

                    App.Current.Properties["jwt"] = token;
                    App.Current.Properties["userId"] = userId;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var main = new MainView(email, token);
                        main.Show();

                        // Ferme la fenêtre actuelle (A2FView)
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.DataContext == this)
                            {
                                window.Close();
                                break;
                            }
                        }
                    });
                }
                else
                {
                    Message = "Code invalide.";
                    Console.WriteLine("Réponse refusée : " + response.StatusCode);
                    Console.WriteLine("Détails : " + responseBody);
                }
            }
            catch (Exception ex)
            {
                Message = "Erreur lors de la vérification.";
                Console.WriteLine("Exception : " + ex.Message);
            }
        }
    }
}
