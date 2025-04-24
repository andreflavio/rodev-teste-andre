using RO.DevTest.Domain.Entities;
using RO.DevTest.Persistence;
using RO.DevTest.Application.Contracts.Persistance.Repositories;

namespace RO.DevTest.Persistence.Repositories
{
    public class VendaRepository : BaseRepository<Venda>, IVendaRepository
    {
        public VendaRepository(DefaultContext context) : base(context)
        {
        }
    }
}
