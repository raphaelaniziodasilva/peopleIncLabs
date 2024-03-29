using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
        [Range(1, 150)]
        public int Age { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

    }
}   
