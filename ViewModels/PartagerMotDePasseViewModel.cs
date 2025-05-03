using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using coffre_fort2.Services;
using System.Windows.Input;
using System.Windows;
using coffre_fort2.Models;
using System.Net.Http;
using System.Text.Json;

namespace coffre_fort2.ViewModels
{
    public class PartagerMotDePasseViewModel : BaseViewModel
    {
        private readonly PasswordService _passwordService;
        private readonly PasswordEntry _entree;
        private readonly Action _fermer;

        public PartagerMotDePasseViewModel(PasswordService passwordService, int utilisateurId, Action fermer, string jwtToken)
        {
            _passwordService = passwordService;
           
            _fermer = fermer;
            _utilisateurId = utilisateurId;

            _httpClient = new HttpClient();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7182/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            PartagerCommande = new RelayCommand(async _ => await PartagerAsync());
            AnnulerCommande = new RelayCommand(_ => _fermer());
        }


        public string NomUtilisateurCible { get; set; }
        private readonly int _utilisateurId;


        public ICommand PartagerCommande { get; }
        public ICommand AnnulerCommande { get; }
        private readonly HttpClient _httpClient;

        private async Task PartagerAsync()
        {
            if (string.IsNullOrWhiteSpace(NomUtilisateurCible))
            {
                MessageBox.Show("Veuillez saisir un nom d'utilisateur.");
                return;
            }

            try
            {
                // Requête pour obtenir l'utilisateur cible
                var response = await _httpClient.GetAsync($"api/user/by-identifiant/{NomUtilisateurCible}");

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Utilisateur cible introuvable.");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (user == null)
                {
                    MessageBox.Show("Erreur lors de la recuperation de l'utilisateur.");
                    return;
                }

                // Construction de l'objet de partage
                var partage = new PasswordShare
                {
                    SourceUserId = _utilisateurId,   // ID de celui qui partage ses donnes
                    SharedWithUserId = user.Id,
                    DatePartage = DateTime.UtcNow
                };


                // Appel a l’API REST
                MessageBox.Show($"PARTAGE\nSourceUserId = {_utilisateurId}\nTargetUser = {NomUtilisateurCible}");

                await _passwordService.PartagerMotDePasseAsync(partage);

                // Confirmation et fermeture
                MessageBox.Show("Mot de passe partage avec succes.");
                _fermer();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur pendant le partage : " + ex.Message);
            }
        }


    }
}
