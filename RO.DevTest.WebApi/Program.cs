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

// No arquivo Program.cs (substitua o conteúdo atual por este)

// >>> ADICIONE ESTES USINGS SE AINDA NÃO ESTIVEREM <<<
using Microsoft.AspNetCore.Authentication.JwtBearer; // Necessário para JwtBearerDefaults
using Microsoft.IdentityModel.Tokens; // Necessário para TokenValidationParameters, SymmetricSecurityKey
using System.Text; // Necessário para Encoding.UTF8
using Microsoft.Extensions.Options; // Necessário para IOptions (se usar IOptions)
// -------------------------------------------------

using RO.DevTest.Application;
using RO.DevTest.Infrastructure;
using RO.DevTest.Infrastructure.IoC; // Assumindo que a injeção de infraestrutura está aqui
using RO.DevTest.Persistence;
using RO.DevTest.Persistence.IoC; // Assumindo que a injeção de persistência está aqui
using Microsoft.EntityFrameworkCore; // Adicionado para DbContext.Database.EnsureCreated/Migrate (mantenha se usar)
// Usings para repositórios específicos (mantenha se necessário, mas a injeção via IoC já deve cuidar disso)
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence.Repositories;
// Adicione usings que possam estar faltando no seu código original mas que são necessários

namespace RO.DevTest.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Adicione a configuração ao contêiner de serviços se precisar acessá-la em outros lugares (opcional)
        // builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

        // Configuração dos Serviços (Seus serviços existentes)
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configuração do CORS - INÍCIO (Sua configuração existente)
        builder.Services.AddCors(options =>
        {
            // Política de CORS mais específica (pode ser útil para outros ambientes ou cenários)
            options.AddPolicy("AllowSwaggerUI",
                builder =>
                {
                    // Permite requisições vindas especificamente da origem onde o Swagger UI está rodando via HTTP/HTTPS
                    // Ajuste a porta (5087) se necessário.
                    builder.WithOrigins("http://localhost:5087", "https://localhost:5087") // Adicionado HTTPS também
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

        // Configuração de Injeção de Dependência das Camadas (Suas chamadas existentes)
        // Estas linhas devem configurar DbContext, Repositórios base e específicos, IIdentityAbstractor, IJwtTokenGenerator, etc.
        // Verifique se elas já incluem a configuração para todos os seus repositórios (Cliente, Produto, Venda, User)
        // e as abstrações (IdentityAbstractor, JwtTokenGenerator).
        builder.Services.InjectPersistenceDependencies(builder.Configuration);
        builder.Services.InjectInfrastructureDependencies(); // <-- Assumindo que JwtTokenGenerator e IdentityAbstractor são registrados aqui


        // ** REGISTRO DOS REPOSITÓRIOS ESPECÍFICOS (Sua configuração - mantenha se necessária) **
        // Se InjectPersistenceDependencies já registra IClienteRepository, IProdutoRepository, IVendaRepository,
        // e IVendaRepository, você pode REMOVER as linhas abaixo para evitar registros duplicados.
        // Verifique o código dos seus métodos Inject...Dependencies.
        builder.Services.AddScoped<IClienteRepository, ClienteRepository>(); // Registro do repositório IClienteRepository
        builder.Services.AddScoped<IBaseRepository<Cliente>, BaseRepository<Cliente>>(); // Caso o BaseRepository esteja sendo utilizado para outras operações
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();  // Registro do repositório de Produto
        builder.Services.AddScoped<IVendaRepository, VendaRepository>();     // Registro do repositório de Venda


        // Configuração do MediatR (Sua configuração existente)
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly, // Assembly da camada de Application
                typeof(Program).Assembly // Assembly da camada WebApi
            );
        });

        // *** CONFIGURAÇÃO JWT (Integrada e Corrigida) ***

        // 1. Binda a seção "JwtSettings" do appsettings.json à classe JwtSettings
        // ESTA LINHA É CRUCIAL para que a configuração seja lida corretamente.
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        // Opcional: Verificar configurações JwtSettings no startup (boa prática)
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
                // Esta configuração instrui o middleware JwtBearer como validar o token recebido
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, // O token deve ter um emissor
                    ValidateAudience = true, // O token deve ter uma audiência
                    ValidateLifetime = true, // O token deve ter uma data de expiração e não estar expirado
                    ValidateIssuerSigningKey = true, // A assinatura do token deve ser válida

                    // >>> CORREÇÃO AQUI: Ler da configuração USANDO AS CHAVES CORRESPONDENTES À SEÇÃO "JwtSettings" <<<
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // O emissor válido (ex: "seuapi.com")
                    ValidAudience = builder.Configuration["JwtSettings:Audience"], // A audiência válida (ex: "seufrontend.com")
                    // A chave secreta para verificar a assinatura. Deve ser a MESMA usada para gerar o token.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
                    // -------------------------------------------------------------------------------
                };
                // Opcional: Configurar o tratamento de eventos JWT (para logs, customização de respostas, etc.)
                // options.Events = new JwtBearerEvents
                // {
                //     OnAuthenticationFailed = context => { /* Logar falha */ return Task.CompletedTask; },
                //     OnTokenValidated = context => { /* Customizar Claims */ return Task.CompletedTask; }
                // };
            });

        // 3. Adicionar Serviços de Autorização
        builder.Services.AddAuthorization();

        // *** FIM DA CONFIGURAÇÃO JWT ***


        var app = builder.Build(); // Linha que constrói a aplicação

        // --- Opcional: Aplicar Migrations automaticamente na inicialização (Sua lógica - mantenha se usar) ---
        // Certifique-se de que esta lógica está correta para seu DbContext e Migrations
        // using (var scope = app.Services.CreateScope())
        // {
        //     var services = scope.ServiceProvider;
        //     try
        //     {
        //         var dbContext = services.GetRequiredService<DefaultContext>(); // Use o tipo real do seu DbContext
        //         if (dbContext.Database.GetPendingMigrations().Any())
        //         {
        //             dbContext.Database.Migrate();
        //         }
        //         // Opcional: Criar o banco de dados se ele não existir (menos comum com Migrations)
        //         // dbContext.Database.EnsureCreated();
        //     }
        //     catch (Exception ex)
        //     {
        //         var logger = services.GetRequiredService<ILogger<Program>>(); // Adicione ILogger se necessário
        //         logger.LogError(ex, "An error occurred while migrating or creating the database.");
        //         // Tratar erro na inicialização, talvez sair da aplicação
        //     }
        // }
        // --- Fim da aplicação automática de Migrations ---


        // Configuração do Pipeline de Requisição HTTP
        // Esta seção define a ordem em que os middlewares processam as requisições.

        // Middleware para tratamento de exceções (geralmente o primeiro)
        // app.UseExceptionHandler("/Error"); // Para ambientes que não são de desenvolvimento
        // app.UseHsts(); // Para ambientes de produção (HSTS - HTTP Strict Transport Security)

        // ** Configurações ESPECÍFICAS para o Ambiente de Desenvolvimento **
        if (app.Environment.IsDevelopment())
        {
            // Habilita o Swagger e Swagger UI apenas em desenvolvimento
            app.UseSwagger();
            app.UseSwaggerUI();

            // Adicionar middleware de página de exceção do desenvolvedor (útil em dev)
            // app.UseDeveloperExceptionPage(); // Mantenha se usar

            // ** HABILITA A POLÍTICA DE CORS PERMISSIVA APENAS EM DESENVOLVIMENTO **
            // Coloque UseCors AQUI para que ele seja aplicado no pipeline de DEV.
            // app.UseRouting() DEVE vir ANTES de UseCors se o CORS precisar de informações de rota.
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

        // --- Middlewares COMUNS a TODOS os ambientes (Geralmente) ---

        // UseRouting deve vir ANTES de UseCors, UseAuthentication, UseAuthorization.
        // Ele seleciona o endpoint, o que é necessário antes de aplicar políticas de CORS ou verificar autorização.
        app.UseRouting(); // <-- Boa prática ter explícito e antes de CORS/Auth

        // O middleware de CORS deve vir APÓS UseRouting, pois a política pode depender da rota.
        // Se você tem UseCors DENTRO dos blocos if/else como acima, remova-o daqui.
        // Se você moveu UseCors para FORA dos if/else, coloque-o aqui APÓS UseRouting.
        // app.UseCors("NomeDaPoliticaParaTodosOsAmbientes"); // Exemplo: se usar a mesma política para todos

        // Middlewares de Autenticação e Autorização SÃO COMUNS a TODOS os ambientes onde você quer segurança.
        // Eles DEVEM vir APÓS UseRouting e APÓS UseCors (se usar CORS)
        app.UseAuthentication(); // Essencial: Adicionado para usar a autenticação configurada (lê o token)
        app.UseAuthorization(); // Essencial: Adicionado para usar a autorização (verifica [Authorize] e papéis)

        // MapControllers mapeia os endpoints dos seus controladores.
        // Deve vir DEPOIS dos middlewares que afetam a seleção/autorização de endpoint (Routing, CORS, Auth).
        app.MapControllers();

        // Middlewares de fallback ou terminais (rodam no final se nenhuma rota anterior lidou com a requisição)
        // app.MapFallbackToFile("index.html"); // Exemplo para SPAs

        // Inicia a aplicação
        app.Run();
    }
}