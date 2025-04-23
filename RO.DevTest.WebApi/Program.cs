using RO.DevTest.Application;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence;
using RO.DevTest.Persistence.IoC;
using Microsoft.EntityFrameworkCore; // Adicionado para DbContext.Database.EnsureCreated/Migrate
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
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

        // ** ADICIONE ESTA LINHA AQUI PARA REGISTRAR O IBaseRepository<Cliente> **
        builder.Services.AddScoped(typeof(IBaseRepository<Cliente>), typeof(BaseRepository<Cliente>));

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