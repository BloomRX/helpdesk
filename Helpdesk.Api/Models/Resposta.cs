using System;
using Helpdesk.Api.Models;


public class Resposta
{
    public int Id { get; set; }
    public required string Conteudo { get; set; } = string.Empty;
    public string EmailUsuario { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public int SolicitacaoId { get; set; }
    public Solicitacao Solicitacao { get; set; } = null!;
    public bool Melhor { get; set; } = false;
}
