using Microsoft.Extensions.Options; // <<< ADICIONE ESTE USING
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application.Contracts.Infrastructure; // Verifique se este using está correto
using RO.DevTest.Domain.Entities; // Verifique se User está aqui
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RO.DevTest.Application.Features.Auth; // <<< ADICIONE ESTE USING (Assumindo que JwtSettings está aqui, ajuste se for em outra camada/pasta)


namespace RO.DevTest.Infrastructure // Namespace ajustado, mantenha este
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        // <<< MUDE DE IConfiguration PARA IOptions<JwtSettings> >>>
        private readonly IOptions<JwtSettings> _jwtSettingsOptions;

        // <<< MUDE O CONSTRUTOR PARA RECEBER IOptions<JwtSettings> >>>
        public JwtTokenGenerator(IOptions<JwtSettings> jwtSettingsOptions)
        {
            _jwtSettingsOptions = jwtSettingsOptions;
        }

        public string GenerateToken(User user, IList<string> roles)
        {
            // <<< OBTENHA AS CONFIGURAÇÕES AQUI USANDO .Value >>>
            var jwtSettings = _jwtSettingsOptions.Value;

            // Uma verificação de segurança (embora a validação na inicialização deva pegar)
            if (string.IsNullOrEmpty(jwtSettings.Secret))
            {
                throw new InvalidOperationException("Configuração da chave secreta JWT (JwtSettings:Secret) está faltando ou vazia.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // <<< ACESSE AS CONFIGURAÇÕES CORRETAMENTE USANDO jwtSettings.NomeDaPropriedade >>>
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)); // Use jwtSettings.Secret
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Use ExpiryMinutes da configuração para o tempo de expiração
            var expirationTime = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime, // Use o tempo de expiração calculado
                Issuer = jwtSettings.Issuer, // Use jwtSettings.Issuer
                Audience = jwtSettings.Audience, // Use jwtSettings.Audience
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}