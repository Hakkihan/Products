using System.ComponentModel.DataAnnotations;

namespace Products.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "password is required")]
        public string? Password { get; set; }
    }
}
