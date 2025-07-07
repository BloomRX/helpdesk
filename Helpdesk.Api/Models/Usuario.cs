namespace Helpdesk.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }             // Será chave primária no banco
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }       
    }
}
