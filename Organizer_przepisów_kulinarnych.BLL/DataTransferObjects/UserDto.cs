namespace Organizer_przepisów_kulinarnych.BLL.DataTransferObjects
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Username { get; set; } 
        public required string FirstName { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }

    }
}
