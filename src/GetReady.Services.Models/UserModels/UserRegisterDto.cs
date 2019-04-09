using System.ComponentModel.DataAnnotations;

namespace GetReady.Services.Models.UserModels
{
    public class UserRegisterDto
    {
        [MinLength(3, ErrorMessage ="Username must be at least three characters long!"), Required]
        public string Username { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least six caracters long!"), Required]
        public string  Password { get; set; }

        [MinLength(6, ErrorMessage = "Repeat Password must be at least six caracters long!"), Required]
        public string  RepeatPassword { get; set; }

        [MinLength(2, ErrorMessage = "First Name must be at least two character long!"), Required]
        public string  FirstName { get; set; }

        [MinLength(2, ErrorMessage = "Last Name must be at least two character long!"), Required]
        public string  LastName { get; set; }
    }
}
