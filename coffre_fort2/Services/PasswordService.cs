using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using coffre_fort2.Models;

namespace coffre_fort2.Services
{
    public class PasswordService
    {
        private readonly HttpClient _httpClient;

        public string JwtToken { get; }

        public PasswordService(string jwtToken)
        {
            JwtToken = jwtToken;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7182") // adapte le port si besoin
            };
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtToken);
        }

        public async Task<List<PasswordEntry>> GetByUserIdAsync(int userId)
        {
            try
            {
                var endpoint = $"api/password/user/{userId}";
                Console.WriteLine(">>> Appel GET vers : " + endpoint);

                var response = await _httpClient.GetAsync(endpoint);
                Console.WriteLine(">>> Code HTTP : " + response.StatusCode);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(">>> Reponse JSON : " + json);

                return JsonSerializer.Deserialize<List<PasswordEntry>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur pendant GetByUserIdAsync : " + ex.Message);
                throw;
            }
        }


        public async Task<PasswordEntry> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/password/{id}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PasswordEntry>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<PasswordEntry> CreateAsync(PasswordEntry entry)
        {
            var json = JsonSerializer.Serialize(entry);
            Console.WriteLine("=== Donnees envoyees a l'API ===");
            Console.WriteLine(json);
            Console.WriteLine("================================");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/password", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Erreur API : " + errorContent);
                throw new Exception("Erreur API: " + errorContent);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PasswordEntry>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/password/{id}");
            return response.IsSuccessStatusCode;
        }



        public async Task<PasswordEntry> UpdateAsync(PasswordEntry entry)
        {
            var json = JsonSerializer.Serialize(entry);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/api/password/{entry.Id}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PasswordEntry>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        public async Task PartagerMotDePasseAsync(PasswordShare partage)
        {
            var response = await _httpClient.PostAsJsonAsync("api/password/share", partage);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erreur API : {response.StatusCode} - {error}");
            }
        }

        public async Task<List<PasswordEntry>> GetSharedPasswords(int userId)
        {
            var response = await _httpClient.GetAsync($"api/passwords/shared/{userId}");
            response.EnsureSuccessStatusCode();

            var shared = await response.Content.ReadFromJsonAsync<List<PasswordEntry>>();
            return shared ?? new List<PasswordEntry>();
        }
        public async Task<List<PasswordEntry>> GetAllSharedPasswordsAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/password/shared-from-users/{userId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<PasswordEntry>>() ?? new List<PasswordEntry>();
        }



    }
}
