using System;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class Solicitacao
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descricao { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public string UsuarioEmail { get; set; }
    }
}
