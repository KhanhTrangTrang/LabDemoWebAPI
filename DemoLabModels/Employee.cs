using System.ComponentModel.DataAnnotations;

namespace DemoLabModels
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Hãy nhập tên")]
        [RegularExpression(@"^\w((?!(<\S)|(\S>)).)*$", ErrorMessage = "Tên không đúng format")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Hãy nhập email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail không đúng theo format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hãy nhập số tel")]
        [RegularExpression(@"^(?=.{0,14}$)([0-9]+)[-. ]([0-9]+)[-. ]([0-9]+)", ErrorMessage = "Tel không đúng như format")]
        public string Tel { get; set; }
    }
}
