using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence
{
    public class DefaultContext : IdentityDbContext<User> // Mantém o DbContext do Identity para usuários
    {
        // Construtores
        public DefaultContext() { }

        public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }

        // DbSet para as tabelas de entidades
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }  // Adicionando ItemVenda

        // Configuração do modelo
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Extensão para gerar UUIDs
            builder.HasPostgresExtension("uuid-ossp");

            // Aplica as configurações do modelo a partir do assembly do DefaultContext
            builder.ApplyConfigurationsFromAssembly(typeof(DefaultContext).Assembly);

            // Configurações para a tabela de clientes
            builder.Entity<Cliente>().ToTable("Clientes");

            // Configuração explícita para a relação entre ItemVenda, Produto e Venda
            builder.Entity<ItemVenda>()
                .HasOne(i => i.Produto)
                .WithMany()  // Caso um Produto possa aparecer em múltiplos ItensVenda
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade); // Se necessário, altere o comportamento de deleção

            builder.Entity<ItemVenda>()
                .HasOne(i => i.Venda)
                .WithMany(v => v.Itens)  // Supondo que uma Venda tenha vários Itens
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);  // Deleção em cascata, se necessário

            builder.Entity<Venda>()
                .HasOne(v => v.Cliente)
                .WithMany()  // Caso um Cliente possa ter várias Vendas
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);  // Ajuste conforme seu comportamento de deleção (pode ser Cascade ou Restrict)

            // Certifique-se de aplicar as configurações necessárias para Produto e Venda se necessário
            base.OnModelCreating(builder);
        }
    }
}
