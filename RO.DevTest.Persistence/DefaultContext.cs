using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities; // Certifique-se de que 'Cliente' está nesta namespace

namespace RO.DevTest.Persistence
{
    public class DefaultContext : IdentityDbContext<User> // Mantém o DbContext do Identity para usuários
    {
        // Construtores
        public DefaultContext() { }

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }

        // DbSet para a tabela de clientes
        public DbSet<Cliente> Clientes { get; set; }

        // Configuração do modelo
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Extensão para gerar UUIDs
            builder.HasPostgresExtension("uuid-ossp");

            // Aplica as configurações do modelo a partir do assembly do DefaultContext
            builder.ApplyConfigurationsFromAssembly(typeof(DefaultContext).Assembly);

            // Configurações adicionais para a tabela de clientes, se necessário
            builder.Entity<Cliente>().ToTable("Clientes"); // Nome da tabela no banco, se necessário

            base.OnModelCreating(builder);
        }
    }
}
