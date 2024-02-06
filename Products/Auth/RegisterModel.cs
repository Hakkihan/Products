using System.ComponentModel.DataAnnotations;

namespace Products.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "username is required")]
        [RegularExpression("^[a-zA-Z]+( [a-zA-Z]+)*$", ErrorMessage = "Please only use characters a-z, A-Z with no spaces.")]
        public string? Username { get; set; }
        [EmailAddress]
        public string? EmailAddress { get; set;}


        [Required(ErrorMessage = "password is required")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,50}$", ErrorMessage = "At least one of each required: uppercase, lowercase, digit, special character with length 8-50 characters. ")]
        
        public string? Password { get; set;}    

    }
}
