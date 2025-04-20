using RO.DevTest.Application;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence;
using RO.DevTest.Persistence.IoC;


namespace RO.DevTest.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adicione a configuração ao contêiner de serviços se precisar acessá-la em outros lugares
        // builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // --- ALTERAÇÃO AQUI ---
        // Chame o método InjectPersistenceDependencies e PASSE a configuração (builder.Configuration)
        builder.Services.InjectPersistenceDependencies(builder.Configuration)
            .InjectInfrastructureDependencies();
        // --- FIM DA ALTERAÇÃO ---


        // Add Mediatr to program
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            );
        });

        var app = builder.Build();

        // --- Opcional: Aplicar Migrations automaticamente na inicialização (bom para desenvolvimento/testes) ---
        // Esta parte não é estritamente necessária para RODAR a app, mas garante que o DB esteja atualizado
        // Pode ser removido em ambientes de produção mais controlados.
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
            // Se estiver usando um banco de dados de verdade, certifique-se de que o banco existe
            // dbContext.Database.Migrate(); // Aplica as migrations pendentes
            // Ou para recriar o banco a cada run (apenas DEV/TESTES MUITO INICIAIS)
            dbContext.Database.EnsureCreated(); // Cuidado! Isso não usa migrations e pode ser problemático
        }
        // --- Fim da seção opcional de Migrations ---


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}