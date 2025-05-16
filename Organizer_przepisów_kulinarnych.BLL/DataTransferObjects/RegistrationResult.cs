using Organizer_przepisów_kulinarnych.DAL.Entities;

namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class RegistrationResult
    {
            public bool Success { get; set; }
            public string? ErrorMessage { get; set; }
            public User? User { get; set; }

            public static RegistrationResult Failed(string error) =>
                new RegistrationResult { Success = false, ErrorMessage = error };

            public static RegistrationResult Successful(User user) =>
                new RegistrationResult { Success = true, User = user };
    }
}
