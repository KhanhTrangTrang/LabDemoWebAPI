using System.ComponentModel.DataAnnotations;

namespace DemoLabModels
{
    public class Admin
    {
        [Required(ErrorMessage = "Hãy nhập ID")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập password")]
        public string Password { get; set; }
    }
}
