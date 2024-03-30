using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace peopleIncLabs.Models
{
    public class Person
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(50, ErrorMessage = "O tamanho do nome não pode exceder 50 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A idade é obrigatória")]
        [Range(1, 150, ErrorMessage = "A idade deve estar entre 1 e 150 anos")]
        public int Age { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [MaxLength(150, ErrorMessage = "O tamanho do e-mail não pode exceder 150 caracteres")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

    }
}   
