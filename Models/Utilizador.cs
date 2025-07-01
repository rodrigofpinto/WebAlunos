using System;
using System.ComponentModel.DataAnnotations;

namespace WebAlunos.Models
{
    public class Utilizador
    {
        public int NUtilizador { get; set; } 

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "E-Mail")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "A password é obrigatória")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
