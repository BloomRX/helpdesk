using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Helpdesk.Api.Models;

namespace Helpdesk.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=helpdesk.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
