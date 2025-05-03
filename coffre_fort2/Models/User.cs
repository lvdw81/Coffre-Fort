using System.Text.Json.Serialization;

namespace coffre_fort2.Models
{
    public class User
    {
        public int Id { get; set; }

        [JsonPropertyName("identifiant")]
        public string Identifiant { get; set; }

        [JsonPropertyName("motDePasseHash")]
        public string MotDePasseHash { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}
