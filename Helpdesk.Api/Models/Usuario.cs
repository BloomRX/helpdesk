namespace Helpdesk.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }             // Será chave primária no banco
public required string Nome { get; set; }
public required string Email { get; set; }
public required string Senha { get; set; }

    }
}
