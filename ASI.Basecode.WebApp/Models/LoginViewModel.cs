using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ASI.Basecode.WebApp.Models
{
    /// <summary>
    /// Login View Model
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>ユーザーID</summary>
        [JsonPropertyName("userName")]
        [Required(ErrorMessage = "Username is required!")]
        public string UserName { get; set; }
        /// <summary>パスワード</summary>
        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
    }
}
