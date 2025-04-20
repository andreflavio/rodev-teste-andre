namespace RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand;

public record GetAllUsersResult
{
    public string Id { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public GetAllUsersResult(object user) { }

    public GetAllUsersResult(Domain.Entities.User user)
    {
        Id = user.Id;
        UserName = user.UserName ?? string.Empty;
        Name = user.Name ?? string.Empty;
        Email = user.Email ?? string.Empty;
    }
}
