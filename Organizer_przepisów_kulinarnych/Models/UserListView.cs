using System.ComponentModel.DataAnnotations;

namespace Organizer_przepisów_kulinarnych.Models
{
    public class UserListView
    {
        public int Id { get; set; }

        
        public string Username { get; set; } = string.Empty;

        
        public string FirstName { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

       
        public string Email { get; set; } = string.Empty;
    }
}
