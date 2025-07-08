public class Resposta
{
    public int Id { get; set; }
    public string Conteudo { get; set; }
    public string EmailUsuario { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public int SolicitacaoId { get; set; }
    public Solicitacao Solicitacao { get; set; }
}
