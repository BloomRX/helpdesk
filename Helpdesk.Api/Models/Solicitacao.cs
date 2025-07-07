using System;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class Solicitacao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título é obrigatório")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Descricao { get; set; } = string.Empty;

        public DateTime DataAbertura { get; set; } = DateTime.Now;

        public string UsuarioEmail { get; set; } = string.Empty;

        //public StatusSolicitacao Status { get; set; } = StatusSolicitacao.Aberto;
    }
}
