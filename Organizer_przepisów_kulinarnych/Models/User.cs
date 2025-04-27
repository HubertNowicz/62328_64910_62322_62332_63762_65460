using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Organizer_przepisów_kulinarnych.Models
{
    public class User
    {
    [Key]
     public int Id { get; set; }

     [Required(ErrorMessage = "Imie jest wymagana.")]
     [StringLength(100)]
     public string UserName { get; set; }

     [Required(ErrorMessage = "Nazwisko jest wymagana.")]
     [StringLength(100)]
     public string Surname { get; set; }

     [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
     [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
     public string Email { get; set; }
     
     [Required(ErrorMessage = "Hasło jest wymagane.")]
     [StringLength(100, ErrorMessage = "Hasło musi mieć co najmniej {2} znaków.", MinimumLength = 6)]
     public string Password { get; set; }

       [Required]
     public int UserRoleId { get; set; }
        
     public virtual UserRole UserRole { get; set; }

    }
}
