using System.ComponentModel.DataAnnotations;

namespace Authentication_Authorization_Platform___IAM.Models
{
    public class LoginRequest
    {


        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = null!;

    }
}
