using System;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class Solicitacao
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public string EmailUsuario { get; set; } = string.Empty;

        public DateTime DataAbertura { get; set; } = DateTime.Now;

        
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        public StatusSolicitacao Status { get; set; } = StatusSolicitacao.Aberto;
        public List<Resposta> Respostas { get; set; } = new();
    }
}

