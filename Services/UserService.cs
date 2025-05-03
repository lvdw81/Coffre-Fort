using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using coffre_fort2.Models;

namespace coffre_fort2.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(string jwtToken)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7182")
            };
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        public async Task<User> GetUserByIdentifiantAsync(string identifiant)
        {
            var response = await _httpClient.GetAsync($"/api/user/by-identifiant/{identifiant}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
