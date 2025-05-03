using System.Collections.Generic;
using System.Text.Json.Serialization;
using coffre_fort2.ViewModels;

namespace coffre_fort2.Models
{
    public class PasswordEntry : BaseViewModel
    {
        private int _id;
        [JsonPropertyName("id")]
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _nomApplication = "";
        [JsonPropertyName("NomApplication")] 
        public string NomApplication
        {
            get => _nomApplication;
            set => SetProperty(ref _nomApplication, value);
        }


        private string _identifiant = "";
        [JsonPropertyName("identifiant")]
        public string Identifiant
        {
            get => _identifiant;
            set => SetProperty(ref _identifiant, value);
        }

        private string _motDePasse = "";
        [JsonPropertyName("motDePasse")]
        public string MotDePasse
        {
            get => _motDePasse;
            set
            {
                SetProperty(ref _motDePasse, value);
                OnPropertyChanged(nameof(MotDePasseVisible));
            }
        }

        private List<string> _tags = new();
        [JsonPropertyName("tags")]
        public List<string> Tags
        {
            get => _tags;
            set
            {
                SetProperty(ref _tags, value);
                OnPropertyChanged(nameof(TagsAffiche));
            }
        }

        private string _utilisateur = "";
        public string Utilisateur
        {
            get => _utilisateur;
            set => SetProperty(ref _utilisateur, value);
        }

        private int _utilisateurId;
        [JsonPropertyName("userId")]
        public int UtilisateurId
        {
            get => _utilisateurId;
            set => SetProperty(ref _utilisateurId, value);
        }

        [JsonIgnore]
        private bool _afficherMotDePasse = false;

        [JsonIgnore]
        public bool AfficherMotDePasse
        {
            get => _afficherMotDePasse;
            set
            {
                SetProperty(ref _afficherMotDePasse, value);
                OnPropertyChanged(nameof(MotDePasseVisible));
            }
        }

        [JsonIgnore]
        public string MotDePasseVisible => AfficherMotDePasse ? MotDePasse : new string('•', MotDePasse.Length);

        [JsonIgnore]
        public string TagsAffiche => Tags == null || Tags.Count == 0 ? "" : string.Join(", ", Tags);
    }
}
