using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using RO.DevTest.Application;
using RO.DevTest.Infrastructure.IoC;
using RO.DevTest.Persistence.IoC;
using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence.Repositories;
using RO.DevTest.Application.Features.Auth;
using System.Linq;
using RO.DevTest.Infrastructure;
using Microsoft.OpenApi.Models;
using RO.DevTest.WebApi.Middleware;

namespace RO.DevTest.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- Configuração dos Serviços ---

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        // Adicionar Swagger com suporte a JWT
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "RO.DevTest.API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header usando o esquema Bearer.
                                Digite 'Bearer {seu token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });

        // Configuração do CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSwaggerUI",
                builder =>
                {
                    builder.WithOrigins("http://localhost:5087")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

            options.AddPolicy("DevelopmentPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        // Configuração de Injeção de Dependência
        builder.Services.InjectPersistenceDependencies(builder.Configuration)
            .InjectInfrastructureDependencies();

        builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
        builder.Services.AddScoped<IBaseRepository<Cliente>, BaseRepository<Cliente>>();
        builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
        builder.Services.AddScoped<IVendaRepository, VendaRepository>();

        // Configuração do MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationLayer).Assembly,
                typeof(Program).Assembly
            );
        });

        // Mapeamento forte do JwtSettings
        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("JwtSettings"));

        // Validação das configurações do JWT
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (string.IsNullOrEmpty(jwtSettings?.Secret) || jwtSettings.Secret.Length < 32)
            throw new InvalidOperationException("A chave secreta do JWT está ausente ou é muito curta.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]))
                };
            });

        // Configuração de políticas de autorização
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            // Sua lógica de Migrations, se necessário
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("DevelopmentPolicy");
        }
        else
        {
            app.UseHttpsRedirection();
        }

        // Adicionar middleware de tratamento de erros
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}