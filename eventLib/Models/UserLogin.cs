using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eventLib.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "User name is required")]
        [DisplayName("Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DisplayName("Password")]
        public string Password { get; set; } = string.Empty;
    }
}
