using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Domain.Exception;
using Microsoft.AspNetCore.Identity; // Necessário para IdentityResult
using RO.DevTest.Domain.Enums; // Necessário para acessar o enum UserRoles
using RO.DevTest.Domain.Entities; // Necessário para o mock do AssignTo se aplicável

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands;

public class CreateUserCommandHandlerTests
{
    // Removido inicialização no construtor

    [Fact]
    public async Task Handle_WhenEmailIsEmpty_ShouldThrowBadRequestException()
    {
        // Arrange
        var identityAbstractorMock = new Mock<IIdentityAbstractor>();
        var sut = new CreateUserCommandHandler(identityAbstractorMock.Object);

        var command = new CreateUserCommand
        {
            Email = "", // Email vazio para testar validação
            UserName = "user_test",
            Password = "password123",
            PasswordConfirmation = "password123",
            Name = "Test User",
            Role = UserRoles.Customer // Usando membro existente do enum
        };

        // Não precisamos configurar o mock de IdentityAbstractor aqui,
        // pois o teste espera que a validação falhe antes de chamar o IdentityAbstractor.

        // Act
        Func<Task> act = async () => await sut.Handle(command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Handle_WhenPasswordsDoNotMatch_ShouldThrowBadRequestException()
    {
        // Arrange
        var identityAbstractorMock = new Mock<IIdentityAbstractor>();
        var sut = new CreateUserCommandHandler(identityAbstractorMock.Object);

        var command = new CreateUserCommand
        {
            Email = "validemail@domain.com",
            UserName = "user_test",
            Password = "password123",
            PasswordConfirmation = "wrongpassword", // Senhas não coincidem
            Name = "Test User",
            Role = UserRoles.Customer // Usando membro existente do enum
        };

        // Não precisamos configurar o mock de IdentityAbstractor aqui,
        // pois o teste espera que a validação falhe antes de chamar o IdentityAbstractor.

        // Act
        Func<Task> act = async () => await sut.Handle(command, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Handle_WhenValidData_ShouldNotThrowException()
    {
        // Arrange
        var identityAbstractorMock = new Mock<IIdentityAbstractor>();
        var sut = new CreateUserCommandHandler(identityAbstractorMock.Object);

        var command = new CreateUserCommand
        {
            Email = "validemail@domain.com",
            UserName = "user_test",
            Password = "password123",
            PasswordConfirmation = "password123",
            Name = "Test User",
            Role = UserRoles.Customer // Usando membro existente do enum
        };

        // *** CONFIGURAÇÃO DO MOCK PARA ESTE TESTE ***
        identityAbstractorMock
            .Setup(ia => ia.CreateUserAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Configura AddToRoleAsync para retornar um resultado de sucesso
        // CORRIGIDO: Usando It.IsAny<UserRoles>() para o segundo argumento
        identityAbstractorMock
           .Setup(ia => ia.AddToRoleAsync(It.IsAny<Domain.Entities.User>(), It.IsAny<UserRoles>())) // <-- CORREÇÃO AQUI
           .ReturnsAsync(IdentityResult.Success);

        // Lembre-se de garantir que request.AssignTo() funciona ou está mockado para retornar User válido.

        // Act
        Func<Task> act = async () => await sut.Handle(command, new CancellationToken());

        // Assert
        await act.Should().NotThrowAsync();
    }
}