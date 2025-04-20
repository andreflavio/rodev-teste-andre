using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Adicionar este using para IConfiguration

namespace RO.DevTest.Persistence.IoC;

public static class PersistenceDependencyInjector
{
    /// <summary>
    /// Inject the dependencies of the Persistence layer into an
    /// <see cref="IServiceCollection"/>
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to inject the dependencies into
    /// </param>
    /// <param name="configuration"> // Adicionar este parâmetro
    /// The application's configuration
    /// </param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> with dependencies injected
    /// </returns>
    public static IServiceCollection InjectPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // --- ALTERAÇÃO AQUI ---
        // Obtenha a string de conexão da configuração (appsettings.json)
        var connectionString = configuration.GetConnectionString("DefaultConnection"); // Nome da sua string de conexão no appsettings.json

        // Adicione uma verificação básica (opcional, mas recomendável)
        if (string.IsNullOrEmpty(connectionString))
        {
            // Em desenvolvimento, você pode querer uma mensagem de erro clara ou até lançar uma exceção.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n------------------------------------------------------------------------------------");
            Console.WriteLine("ERRO CRÍTICO DE CONFIGURAÇÃO: A string de conexão 'DefaultConnection' para o PostgreSQL não foi encontrada ou está vazia!");
            Console.WriteLine("Por favor, adicione a string de conexão correta no seu arquivo appsettings.json.");
            Console.WriteLine("Exemplo: \"ConnectionStrings\": { \"DefaultConnection\": \"Host=localhost;Port=5432;Database=seu_banco;Username=seu_usuario;Password=sua_senha;\" }");
            Console.WriteLine("------------------------------------------------------------------------------------\n");
            Console.ResetColor();

            // Dependendo de como você quer lidar com isso em caso de erro fatal,
            // você pode querer parar a aplicação:
            // throw new InvalidOperationException("A string de conexão 'DefaultConnection' para o PostgreSQL não está configurada.");

            // Ou, em um cenário de fallback *apenas* para testes locais (não recomendado para ir para outros ambientes),
            // você poderia manter o in-memory SE a string faltar, mas isso é confuso.
            // O ideal é FALHAR se a string do BD principal estiver faltando.

            // Para este exemplo, vamos apenas logar o erro e *não* configurar um DbContext funcional.
            // Isso provavelmente causará erros posteriormente quando a aplicação tentar usar o DbContext.
            // A abordagem de lançar exceção é mais segura para desenvolvimento.
        }
        else
        {
            // Configure o DbContext para usar PostgreSQL
            services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(connectionString, // Use UseNpgsql com a string de conexão
                b => b.MigrationsAssembly(typeof(DefaultContext).Assembly.FullName))); // Adicione isso se usar Migrations no mesmo assembly do DbContext
        }
        // --- FIM DA ALTERAÇÃO ---


        return services;
    }
}