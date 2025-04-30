using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;

namespace RO.DevTest.Infrastructure;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IOptions<JwtSettings> _jwtSettingsOptions;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettingsOptions)
    {
        _jwtSettingsOptions = jwtSettingsOptions ?? throw new ArgumentNullException(nameof(jwtSettingsOptions));
    }

    public object GenerateToken(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var jwtSettings = _jwtSettingsOptions.Value;
        if (string.IsNullOrEmpty(jwtSettings.Secret))
            throw new InvalidOperationException("Configuração da chave secreta JWT (JwtSettings:Secret) está faltando ou vazia.");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.GetDescription())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationTime = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationTime,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token); // Retorna string, compatível com object
    }

    public string GenerateToken(User user, IList<string> roles)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        if (roles == null || roles.Count == 0)
            throw new ArgumentException("Pelo menos um role deve ser fornecido.", nameof(roles));

        var jwtSettings = _jwtSettingsOptions.Value;
        if (string.IsNullOrEmpty(jwtSettings.Secret))
            throw new InvalidOperationException("Configuração da chave secreta JWT (JwtSettings:Secret) está faltando ou vazia.");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, roles[0]) // Usa apenas o primeiro role
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expirationTime = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expirationTime,
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}