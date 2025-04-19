using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace coffre_fort2.Models
{
    public static class UserManager
    {
        private static readonly string UsersFilePath = "users.json";

        public static List<User> ChargerUtilisateurs()
        {
            if (!File.Exists(UsersFilePath))
                return new List<User>();

            var json = File.ReadAllText(UsersFilePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }

        public static void SauvegarderUtilisateurs(List<User> utilisateurs)
        {
            var json = JsonSerializer.Serialize(utilisateurs);
            File.WriteAllText(UsersFilePath, json);
        }

        public static bool AjouterUtilisateur(string identifiant, string motDePasse)
        {
            var utilisateurs = ChargerUtilisateurs();

            if (utilisateurs.Any(u => u.Identifiant == identifiant))
                return false; // utilisateur déjà existant

            var hash = HasherMotDePasse(motDePasse);
            utilisateurs.Add(new User { Identifiant = identifiant, MotDePasseHash = hash });
            SauvegarderUtilisateurs(utilisateurs);
            return true;
        }

        public static bool VerifierConnexion(string identifiant, string motDePasse)
        {
            var utilisateurs = ChargerUtilisateurs();
            var hash = HasherMotDePasse(motDePasse);

            return utilisateurs.Any(u => u.Identifiant == identifiant && u.MotDePasseHash == hash);
        }

        private static string HasherMotDePasse(string motDePasse)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(motDePasse);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
