// No arquivo JwtSettings.cs (dentro do projeto RO.DevTest.Infrastructure)

namespace RO.DevTest.Infrastructure; // <-- Use este namespace se colocar em uma pasta 'Configurations'
// OU:
// namespace RO.DevTest.Infrastructure; // <-- Use este namespace se colocar na raiz do projeto Infrastructure

public class JwtSettings
{
    public string Secret { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int ExpiryMinutes { get; init; }
}