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
        public DbSet<Categoria> Categorias { get; set; } // Categorias de perguntas
        //public DbSet<StatusSolicitacao> StatusSolicitacoes { get; set; } // Status das solicitações


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

    }
}
