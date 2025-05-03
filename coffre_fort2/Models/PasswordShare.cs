namespace coffre_fort2.Models
{
    public class PasswordShare
    {
        public int Id { get; set; }

        public int SourceUserId { get; set; }

        public int SharedWithUserId { get; set; }

        public DateTime DatePartage { get; set; }
    }
}
