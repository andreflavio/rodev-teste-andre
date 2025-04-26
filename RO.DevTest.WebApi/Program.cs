/*
using RO.DevTest.Application;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence;
using RO.DevTest.Persistence.IoC;
using Microsoft.EntityFrameworkCore; // Adicionado para DbContext.Database.EnsureCreated/Migrate
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence.Repositories;

namespace RO.DevTest.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adicione a configuração ao contêiner de serviços se precisar acessá-la em outros lugares
        // builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

        // Configuração dos Serviços
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configuração do CORS - INÍCIO
        builder.Services.AddCors(options =>
        {
            // Política de CORS mais específica (pode ser útil para outros ambientes ou cenários)
            options.AddPolicy("AllowSwaggerUI",
                builder =>
                {
                    // Permite requisições vindas especificamente da origem onde o Swagger UI está rodando via HTTP
                    // Ajuste a porta (5087) se necessário.
                    builder.WithOrigins("http://localhost:5087")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

            // ** POLÍTICA MAIS PERMISSIVA PARA DESENVOLVIMENTO ** 
            // Esta política permite qualquer origem, método e cabeçalho.
            // Use SOMENTE em ambientes de desenvolvimento/teste local.
            options.AddPolicy("DevelopmentPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin() // Permite requisições de QUALQUER origem
                        .AllowAnyHeader() // Permite qualquer cabeçalho na requisição
                        .AllowAnyMethod(); // Permite qualquer método HTTP (GET, POST, PUT, DELETE, etc.)
                });
        });
        // Configuração do CORS - FIM

        // Configuração de Injeção de Dependência das Camadas
        builder.Services.InjectPersistenceDependencies(builder.Configuration)
            .InjectInfrastructureDependencies();

        // ** REGISTRO DO REPOSITÓRIO IClienteRepository ** ADICIONE ESTA LINHA AQUI
        builder.Services.AddScoped<IClienteRepository, ClienteRepository>(); // Registro do repositório IClienteRepository
        builder.Services.AddScoped<IBaseRepository<Cliente>, BaseRepository<Cliente>>(); // Caso o BaseRepository esteja sendo utilizado para outras operações
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();  // Registro do repositório de Produto
        builder.Services.AddScoped<IVendaRepository, VendaRepository>();    // Registro do repositório de Venda


        // Configuração do MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly, // Assembly da camada de Application
                typeof(Program).Assembly // Assembly da camada WebApi
            );
        });

        // ** ADICIONE ESTAS LINHAS AQUI (Configuração da Autenticação JWT) ** 
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"], // Ler do appsettings.json
                    ValidAudience = builder.Configuration["Jwt:Audience"], // Ler do appsettings.json
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Ler do appsettings.json
                };
            });

        builder.Services.AddAuthorization(); // Adicionar autorização
        // ** FIM DA ADIÇÃO **

        var app = builder.Build();

        // --- Opcional: Aplicar Migrations automaticamente na inicialização ---
        using (var scope = app.Services.CreateScope())
        {
            // ... sua lógica de migration ...
        }

        // Configuração do Pipeline de Requisição HTTP
        // Esta seção define a ordem em que os middlewares processam as requisições.

        // ** Configurações ESPECÍFICAS para o Ambiente de Desenvolvimento **
        if (app.Environment.IsDevelopment())
        {
            // Habilita o Swagger e Swagger UI apenas em desenvolvimento
            app.UseSwagger();
            app.UseSwaggerUI();

            // ** HABILITA A POLÍTICA DE CORS PERMISSIVA APENAS EM DESENVOLVIMENTO **
            // Coloque UseCors AQUI para que ele seja aplicado no pipeline de DEV.
            app.UseCors("DevelopmentPolicy"); // <-- Usa a política que permite AllowAnyOrigin()

            // ** DESABILITA o Redirecionamento HTTPS APENAS EM DESENVOLVIMENTO **
            // Comentamos ou removemos UseHttpsRedirection NESTE BLOCO.
            // Isso permite que o Swagger UI (HTTP) se comunique diretamente com a API (ainda que rodando em HTTP/HTTPS)
            // sem o redirecionamento forçado que pode causar conflitos de CORS.
            // app.UseHttpsRedirection(); // <-- Esta linha NÃO deve estar ativa aqui em DEV

        }
        // ** Configurações para OUTROS Ambientes (Produção, Staging, etc.) **
        else
        {
            // Em outros ambientes, o Redirecionamento HTTPS GERALMENTE deve estar ATIVO por segurança.
            app.UseHttpsRedirection();

            // Em outros ambientes, use uma política de CORS MAIS RESTRITIVA e segura, se necessário.
            // Por exemplo, permitir requisições APENAS do seu frontend de produção.
            // app.UseCors("AllowSwaggerUI"); // Exemplo: usar a política mais específica
            // app.UseCors("SuaPoliticaDeProducao");
        }

        // Middlewares COMUNS a TODOS os ambientes (Geralmente)
        // UseRouting deve vir ANTES de UseCors, UseAuthentication, UseAuthorization
        // app.UseRouting(); // Se você não tem UseRouting explícito, MapControllers geralmente o implica

        // O middleware de CORS pode vir aqui também se a mesma política se aplicar a todos os ambientes,
        // mas como temos políticas diferentes para DEV e outros, colocamos dentro dos blocos if/else.
        // Se você moveu UseCors para fora do if/else, remova-o de dentro.
        // app.UseCors("NomeDaPoliticaParaTodosOsAmbientes");

        app.UseAuthentication(); // Adicionado para usar a autenticação configurada
        app.UseAuthorization(); // Adicionado para usar a autorização

        // MapControllers mapeia os endpoints dos seus controladores.
        // Deve vir DEPOIS dos middlewares que afetam a seleção/autorização de endpoint (Routing, CORS, Auth).
        app.MapControllers();

        // Inicia a aplicação
        app.Run();
    }
}
*/
// No arquivo Program.cs

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options; // Necessário para IOptions
using RO.DevTest.Application; // Assumindo ApplicationLayer está aqui
using RO.DevTest.Infrastructure.IoC; // Assumindo injeção de infraestrutura está aqui
using RO.DevTest.Persistence.IoC; // Assumindo injeção de persistência está aqui
using Microsoft.EntityFrameworkCore; // Mantenha se usa Migrations/EnsureCreated
using RO.DevTest.Application.Contracts.Persistence.Repositories; // Mantenha se necessário
using RO.DevTest.Domain.Entities; // Mantenha se necessário (para Cliente na injeção manual)
using RO.DevTest.Persistence.Repositories; // Mantenha se necessário (para ClienteRepository na injeção manual)
using RO.DevTest.Application.Features.Auth; // <<< ADICIONE/MANTENHA ESTE USING (Assumindo que JwtSettings está aqui)
using System.Linq;
using RO.DevTest.Infrastructure; // Mantenha se usa Any() em Migrations

namespace RO.DevTest.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- Configuração dos Serviços ---

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configuração do CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSwaggerUI",
                builder =>
                {
                    // Permite requisições do Swagger UI (HTTP e HTTPS)
                    builder.WithOrigins("http://localhost:5087", "https://localhost:5087")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

            // POLÍTICA MAIS PERMISSIVA PARA DESENVOLVIMENTO (Permite qualquer origem, incluindo http://localhost:8000 do seu frontend simples)
            options.AddPolicy("DevelopmentPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin() // Permite requisições de QUALQUER origem
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        // Configuração de Injeção de Dependência das Camadas (Verifique seus métodos Inject...Dependencies)
        // Essas linhas devem registrar os DbContexts, Repositórios, Identity, JWT Generator, etc.
        builder.Services.InjectPersistenceDependencies(builder.Configuration); // Assumindo que configura DbContext e Repositórios base
        builder.Services.InjectInfrastructureDependencies(); // Assumindo que configura IdentityAbstractor e JwtTokenGenerator

        // Registros de Repositórios Específicos (Mantenha SOMENTE se InjectPersistenceDependencies NÃO os registra)
        // Se seus métodos Inject... JÁ registram IClienteRepository, IProdutoRepository, IVendaRepository, REMOVA as linhas abaixo.
        builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
        builder.Services.AddScoped<IBaseRepository<Cliente>, BaseRepository<Cliente>>();
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
        builder.Services.AddScoped<IVendaRepository, VendaRepository>();


        // Configuração do MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly, // Assembly da camada de Application (Ajuste se o nome da classe for diferente)
                typeof(Program).Assembly // Assembly da camada WebApi
            );
        });


        // *** Configuração de Autenticação JWT ***

        // 1. Binda a seção "JwtSettings" do appsettings.json à classe JwtSettings
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        // Opcional: Verificar configurações JwtSettings no startup (boa prática) - Mantenha se quiser esta validação inicial
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Secret) || string.IsNullOrEmpty(jwtSettings.Issuer) || string.IsNullOrEmpty(jwtSettings.Audience) || jwtSettings.ExpiryMinutes <= 0)
        {
            throw new InvalidOperationException("Configurações JwtSettings inválidas ou ausentes na seção 'JwtSettings' do appsettings.json.");
        }


        // 2. Adicionar Serviços de Autenticação com esquema JWT Bearer
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    // <<< CORRIGIDO: Lendo da configuração USANDO AS CHAVES DA SEÇÃO "JwtSettings" >>>
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // Use "JwtSettings:Issuer"
                    ValidAudience = builder.Configuration["JwtSettings:Audience"], // Use "JwtSettings:Audience"
                    // A chave secreta para verificar a assinatura.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])) // Use "JwtSettings:Secret"
                    // -----------------------------------------------------------------------------------
                };
            });

        // 3. Adicionar Serviços de Autorização
        builder.Services.AddAuthorization();

        // *** FIM DA CONFIGURAÇÃO JWT ***


        var app = builder.Build(); // Linha que constrói a aplicação


        // --- Opcional: Aplicar Migrations automaticamente na inicialização (Mantenha se usar) ---
        // Certifique-se de que esta lógica está correta para seu DbContext e Migrations
        // using (var scope = app.Services.CreateScope())
        // {
        //      // Sua lógica de migration/EnsureCreated aqui...
        // }


        // --- Configuração do Pipeline de Requisição HTTP ---
        // Esta seção define a ordem em que os middlewares processam as requisições.

        // 1. Middleware para tratamento de exceções (geralmente o primeiro)
        // app.UseExceptionHandler("/Error"); // Para ambientes que não são de desenvolvimento
        // app.UseHsts(); // Para ambientes de produção (HSTS - HTTP Strict Transport Security)


        // 2. UseRouting: Deve vir ANTES de CORS, UseAuthentication, UseAuthorization.
        // Ele seleciona o endpoint que irá lidar com a requisição.
        app.UseRouting(); // <<< ADICIONADO OU MOVIDO PARA ESTAR AQUI

        // 3. UseCors: Deve vir APÓS UseRouting
        // Aplica a política de CORS. Se tiver políticas diferentes por ambiente, coloque o UseCors dentro dos blocos if/else.
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("DevelopmentPolicy"); // Aplica política de CORS para DEV (AllowAnyOrigin)
        }
        else
        {
            // app.UseCors("YourProductionPolicy"); // Exemplo: Aplicar política de CORS para Produção
            app.UseHttpsRedirection(); // HTTPS Redirection geralmente fica aqui em ambientes que não são DEV
        }


        // 4. UseAuthentication: Deve vir APÓS UseRouting e UseCors. Autentica o usuário.
        app.UseAuthentication();

        // 5. UseAuthorization: Deve vir APÓS UseAuthentication. Verifica [Authorize] e [AllowAnonymous].
        app.UseAuthorization();


        // 6. MapControllers: Mapeia e executa o código do endpoint selecionado. Deve vir por último.
        app.MapControllers();

        // Middlewares de fallback ou terminais (rodam no final se nenhuma rota anterior lidou com a requisição)
        // app.MapFallbackToFile("index.html"); // Exemplo para SPAs


        // Inicia a aplicação
        app.Run();
    }
}