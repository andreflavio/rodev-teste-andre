using MediatR;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Vendas.Commands;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Application.Features.Vendas.Queries;
using RO.DevTest.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RO.DevTest.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IVendaRepository _vendaRepository;

        public VendaController(IMediator mediator, IVendaRepository vendaRepository)
        {
            _mediator = mediator;
            _vendaRepository = vendaRepository;
        }

        // POST: api/vendas
        [HttpPost]
        public async Task<IActionResult> CreateVenda([FromBody] CreateVendaCommand command)
        {
            if (command == null)
                return BadRequest("Comando de criação de venda não pode ser nulo.");

            var result = await _mediator.Send(command);
            if (result == null)
                return StatusCode(500, "Erro ao criar a venda.");

            return CreatedAtAction(nameof(GetVendaById), new { id = result.VendaId }, result);
        }

        // GET: api/vendas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendaById(Guid id)
        {
            var venda = await _vendaRepository.GetByIdAsync(id);

            if (venda == null)
                return NotFound($"Venda com ID {id} não encontrada.");

            return Ok(venda);
        }

        // GET: api/vendas (Paginação)
        [HttpGet]
        public async Task<IActionResult> GetVendas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Parâmetros de página ou tamanho da página são inválidos.");

            var totalVendas = await _vendaRepository.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalVendas / pageSize);
            var vendas = await _vendaRepository.GetAllAsync(page, pageSize);

            var result = new
            {
                TotalVendas = totalVendas,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Vendas = vendas.Select(v => new GetAllVendasResult
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    DataVenda = v.DataVenda,
                    Total = v.Total,
                    Itens = v.Itens.Select(i => new VendaItemDto
                    {
                        ProdutoId = i.ProdutoId,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario
                    }).ToList()
                }).ToList()
            };

            return Ok(result);
        }

        // PUT: api/vendas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenda(Guid id, [FromBody] UpdateVendaCommand command)
        {
            if (command == null || command.VendaId != id)
                return BadRequest("Dados da venda estão inconsistentes.");

            var vendaExistente = await _vendaRepository.GetByIdAsync(id);
            if (vendaExistente == null)
                return NotFound($"Venda com ID {id} não encontrada.");

            var result = await _mediator.Send(command);
            if (result == null)
                return StatusCode(500, "Erro ao atualizar a venda.");

            return Ok(result);
        }

        // DELETE: api/vendas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(Guid id)
        {
            var vendaExistente = await _vendaRepository.GetByIdAsync(id);
            if (vendaExistente == null)
                return NotFound($"Venda com ID {id} não encontrada.");

            await _vendaRepository.DeleteAsync(vendaExistente);

            return NoContent();
        }
    }
}