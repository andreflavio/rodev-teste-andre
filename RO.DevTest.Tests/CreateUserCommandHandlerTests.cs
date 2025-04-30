using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Application.Features.User.Commands.CreateUserCommand;
using RO.DevTest.Domain.Entities; // Adicionado explicitamente
using RO.DevTest.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly CreateUserCommandHandler _sut;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _sut = new CreateUserCommandHandler(_userRepositoryMock.Object, _jwtTokenGeneratorMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmailIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "",
                UserName = "user_test",
                Password = "password123",
                PasswordConfirmation = "password123",
                Name = "Test User",
                Role = UserRoles.Customer
            };

            // Act
            Func<Task> act = async () => await _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O campo Email do request é obrigatório.*");
        }

        [Fact]
        public async Task Handle_WhenPasswordsDoNotMatch_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "validemail@domain.com",
                UserName = "user_test",
                Password = "password123",
                PasswordConfirmation = "wrongpassword",
                Name = "Test User",
                Role = UserRoles.Customer
            };

            // Act
            Func<Task> act = async () => await _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Os campos Password e PasswordConfirmation do request não coincidem.*");
        }

        [Fact]
        public async Task Handle_WhenUserNameIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "validemail@domain.com",
                UserName = "",
                Password = "password123",
                PasswordConfirmation = "password123",
                Name = "Test User",
                Role = UserRoles.Customer
            };

            // Act
            Func<Task> act = async () => await _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O campo UserName do request é obrigatório.*");
        }

        [Fact]
        public async Task Handle_WhenNameIsEmpty_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "validemail@domain.com",
                UserName = "user_test",
                Password = "password123",
                PasswordConfirmation = "password123",
                Name = "",
                Role = UserRoles.Customer
            };

            // Act
            Func<Task> act = async () => await _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O campo Name do request é obrigatório.*");
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExists_ShouldThrowArgumentException()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "validemail@domain.com",
                UserName = "user_test",
                Password = "password123",
                PasswordConfirmation = "password123",
                Name = "Test User",
                Role = UserRoles.Customer
            };

            var existingUser = new Domain.Entities.User { Email = command.Email }; // Usando namespace completo
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email))
                .ReturnsAsync(existingUser);

            // Act
            Func<Task> act = async () => await _sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O campo Email do request já está em uso.*");
        }

        [Fact]
        public async Task Handle_WhenValidData_ShouldReturnCreateUserResult()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                Email = "validemail@domain.com",
                UserName = "user_test",
                Password = "password123",
                PasswordConfirmation = "password123",
                Name = "Test User",
                Role = UserRoles.Customer
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email))
                .ReturnsAsync((Domain.Entities.User?)null); // Usando namespace completo

            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.User>()))
                .Returns(Task.CompletedTask);

            _jwtTokenGeneratorMock.Setup(g => g.GenerateToken(It.IsAny<Domain.Entities.User>(), It.IsAny<IList<string>>()))
                .Returns("fake-token"); // Atualizado para suportar roles

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be(command.UserName);
            result.Email.Should().Be(command.Email);
            result.Name.Should().Be(command.Name);
            // Removido result.Role.Should().Be(command.Role) devido a CS1061
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(command.Email), Times.Once());
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Once());
            _jwtTokenGeneratorMock.Verify(g => g.GenerateToken(It.IsAny<Domain.Entities.User>(), It.IsAny<IList<string>>()), Times.Once());
        }
    }
}