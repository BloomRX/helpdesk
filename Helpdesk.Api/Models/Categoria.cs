namespace Helpdesk.Api.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;

        // Relação 1:N com Solicitação
        public List<Solicitacao> Solicitacoes { get; set; } = new();
    }
}
