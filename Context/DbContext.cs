using Microsoft.EntityFrameworkCore;
using BancoVirtual.Models;

namespace BancoVirtual.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações de mapeamento e restrições podem ser definidas aqui
            // Por exemplo, você pode configurar chaves primárias, índices, etc.
        }
    }
}
