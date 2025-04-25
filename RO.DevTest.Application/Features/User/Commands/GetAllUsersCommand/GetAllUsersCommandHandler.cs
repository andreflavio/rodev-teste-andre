// No arquivo GetAllUsersCommandHandler.cs
using MediatR;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.GetAllUsersCommand;

public class GetAllUsersCommandHandler : IRequestHandler<GetAllUsersCommand, List<GetAllUsersResult>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<GetAllUsersResult>> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
        // O repositório agora aceita parâmetros de filtro do request
        var users = await _userRepository.GetAllAsync(request.Name, request.UserName); // Passa os filtros do request

        // O mapeamento continua acontecendo aqui no handler
        return users.Select(user => new GetAllUsersResult(user)).ToList();
    }
}