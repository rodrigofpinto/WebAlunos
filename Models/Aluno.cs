using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebAlunos.Models
{
    public class Aluno
    {
        [Required]
        [Display(Name = "Número de aluno")]
        public int NAlunos { get; set; }

        [Required]
        [Display(Name = "Primeiro Nome")]
        public string PrimeiroNome { get; set; }

        [Required]
        [Display(Name = "Último Nome")]
        public string UltimoNome { get; set; }

        [Required]
        public string MoradaNome { get; set; }

        [Required]
        public Sexo Sexo { get; set; }

        [Required]
        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; }

        [Required]
        [Range(1, 12)]
        [Display(Name = "Ano de Escolaridade")]
        public int AnoEscolaridade { get; set; }

        [Display(Name = "Imagem")]
        public string ImagemPath { get; set; }

        public HttpPostedFileBase Imagem { get; set; }
    }
}
