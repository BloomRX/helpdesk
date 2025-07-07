using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty; // Inicialização

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = string.Empty; // Inicialização

        [Required(ErrorMessage = "A senha é obrigatória")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string Senha { get; set; } = string.Empty; // Inicialização

        [NotMapped]
        [Required(ErrorMessage = "Confirme a senha")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; } = string.Empty; // Inicialização

        public TipoUsuario Tipo { get; set; }
    }
}