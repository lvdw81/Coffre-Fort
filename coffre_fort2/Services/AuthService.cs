using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace coffre_fort2.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public string JwtToken { get; private set; }

        public AuthService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7182") // adapte le port si besoin
            };
        }

        // Connexion via l'API
        public async Task<bool> LoginAsync(string identifiant, string motDePasse)
        {
            var loginData = new
            {
                identifiant = identifiant,
                motDePasse = motDePasse
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/api/auth/login", content);

                // Debug : affiche le statut et le corps brut
                Console.WriteLine("Status: " + response.StatusCode);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Réponse brute : " + responseContent);

                if (!response.IsSuccessStatusCode)
                    return false;

                // Plus aucun token ici — juste vérifier que la réponse contient bien l'identifiant ou message attendu
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.Message);
                return false;
            }
        }


        // Creation de compte via l'API
        public async Task<string> RegisterAsync(string identifiant, string motDePasse)
        {
            var userData = new
            {
                identifiant = identifiant,
                motDePasseHash = motDePasse,
                role = "user"
            };

            var json = JsonSerializer.Serialize(userData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("/api/user", content);
                if (response.IsSuccessStatusCode)
                    return "ok";

                var error = await response.Content.ReadAsStringAsync();
                return $"Erreur : {error}";
            }
            catch (Exception ex)
            {
                return "Exception : " + ex.Message;
            }
        }

        public HttpClient GetAuthenticatedClient()
        {
            return _httpClient;
        }
    }
}
