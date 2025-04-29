using MediatR;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;

namespace RO.DevTest.Application.Features.User.Commands.CreateUserCommand;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public CreateUserCommandHandler(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
    }

    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            throw new ArgumentException("O nome de usuário é obrigatório.", nameof(request.UserName));
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("O nome é obrigatório.", nameof(request.Name));
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("O email é obrigatório.", nameof(request.Email));
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("A senha é obrigatória.", nameof(request.Password));
        if (request.Password != request.PasswordConfirmation)
            throw new ArgumentException("A senha e a confirmação da senha não coincidem.", nameof(request.Password));

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new ArgumentException("O email já está em uso.", nameof(request.Email));

        var user = request.AssignTo();
        user.Id = Guid.NewGuid().ToString();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        await _userRepository.AddAsync(user);

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new CreateUserResult(user);
    }
}