using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Adicionar este using para IConfiguration
using System; // Adicionar este using para Console
using RO.DevTest.Application.Contracts.Persistance.Repositories; // Adicionar using para IUserRepository
using RO.DevTest.Persistence.Repositories; // Adicionar using para UserRepository
using RO.DevTest.Persistence; // Adicionar using para DefaultContext (se DefaultContext estiver neste namespace)

namespace RO.DevTest.Persistence.IoC; // Note: O namespace é RO.DevTest.Persistence.IoC

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
        // --- CONFIGURAÇÃO DO DBContext ---
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

            // Em um cenário real, você PROVAVELMENTE quer lançar uma exceção aqui
            // throw new InvalidOperationException("A string de conexão 'DefaultConnection' não está configurada.");

            // Para este exemplo, continuamos, mas o DbContext NÃO será configurado,
            // levando a erros subsequentes quando a aplicação tentar usá-lo.
        }
        else
        {
            // Configure o DbContext para usar PostgreSQL com a string de conexão encontrada
            services.AddDbContext<DefaultContext>(options =>
                 options.UseNpgsql(connectionString,
                 b => b.MigrationsAssembly(typeof(DefaultContext).Assembly.FullName))); // Adicione isso se usar Migrations no mesmo assembly do DbContext
        }
        // --- FIM DA CONFIGURAÇÃO DO DBContext ---

        // --- REGISTRO DOS REPOSITÓRIOS ---
        // ** ESTA É A LINHA ESSENCIAL PARA RESOLVER O SEU ERRO DE DI **
        // Registra a interface IUserRepository para ser resolvida pela classe concreta UserRepository
        services.AddScoped<IUserRepository, UserRepository>(); // <-- Corrigido: UserRepository no segundo tipo

        // Se você tiver outros repositórios específicos (além da BaseRepository),
        // registre-os aqui também. Ex:
        // services.AddScoped<IOtherRepository, OtherRepository>();

        // Note: BaseRepository<TEntity> geralmente não precisa ser registrada diretamente
        // a menos que seja usada diretamente (o que não é comum). Os repositórios específicos
        // que herdam dela (como UserRepository) são os que precisam ser registrados.
        // --- FIM DO REGISTRO DOS REPOSITÓRIOS ---


        return services;
    }
}