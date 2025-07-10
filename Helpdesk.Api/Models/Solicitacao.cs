using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
    public class Solicitacao
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Título é obrigatório.")]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        public string EmailUsuario { get; set; } = string.Empty;

        public DateTime DataAbertura { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria.")]
        public int CategoriaId { get; set; }

        [BindNever]
        public Categoria? Categoria { get; set; }  


        public bool Resolvida { get; set; }
        public DateTime? DataResolucao { get; set; }

        [NotMapped]
        public StatusSolicitacao Status => Resolvida ? StatusSolicitacao.Resolvida : StatusSolicitacao.Aberto;


        public List<Resposta> Respostas { get; set; } = new();
    }
}
