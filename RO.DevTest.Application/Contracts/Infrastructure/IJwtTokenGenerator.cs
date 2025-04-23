using RO.DevTest.Domain.Entities;
using System.Collections.Generic;

namespace RO.DevTest.Application.Contracts.infraes
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IList<string> roles);
    }
}