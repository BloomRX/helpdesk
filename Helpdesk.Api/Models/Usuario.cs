using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        [NotMapped]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; } = string.Empty;
        public string Role { get; set; } = "Usuario";
        
        public string Tipo { get; set; } = "Membro";
    }
}
