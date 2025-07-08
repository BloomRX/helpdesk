using Microsoft.EntityFrameworkCore;
using Helpdesk.Api.Models;

namespace Helpdesk.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }  // Tabela de usuários
        public DbSet<Solicitacao> Solicitacoes { get; set; } // Perguntas Solicitadas
        public DbSet<Resposta> Respostas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

    }
}
