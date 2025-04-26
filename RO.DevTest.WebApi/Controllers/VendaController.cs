// No arquivo VendaController.cs

// Certifique-se de que estes usings estão presentes no topo do arquivo:
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Vendas.Commands;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Application.Features.Vendas.Queries;
using RO.DevTest.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;
// >>> ADICIONE ESTES USINGS TAMBÉM PARA O NOVO ENDPOINT <<<
using RO.DevTest.Application.Features.Vendas.Queries.SalesAnalysis; // Necessário para a Query e Resultado
// -------------------------------------------------------------

namespace RO.DevTest.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IVendaRepository _vendaRepository; // Mantenha este, pois é usado nos Gets/Deletes diretos

        public VendaController(IMediator mediator, IVendaRepository vendaRepository)
        {
            _mediator = mediator;
            _vendaRepository = vendaRepository;
        }

        // POST: api/Venda - CreateVenda
        [HttpPost]
        public async Task<IActionResult> CreateVenda([FromBody] CreateVendaCommand command)
        {
            if (command == null)
                return BadRequest("Comando de criação de venda não pode ser nulo.");

            var result = await _mediator.Send(command);
            if (result == null || !result.Success)
                return BadRequest(result?.Message ?? "Erro ao criar a venda.");

            return CreatedAtAction(nameof(GetVendaById), new { id = result.VendaId }, new
            {
                result.Success,
                result.VendaId,
                result.Message
            });
        }

        // GET: api/Venda/{id} - GetVendaById
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendaById(Guid id)
        {
            var venda = await _vendaRepository.GetByIdAsync(id);

            if (venda == null)
                return NotFound($"Venda com ID {id} não encontrada.");

            return Ok(venda);
        }

        // GET: api/Venda (Paginação) - GetVendas
        [HttpGet]
        public async Task<IActionResult> GetVendas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest("Parâmetros de página ou tamanho da página são inválidos.");

            var totalVendas = await _vendaRepository.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalVendas / pageSize);
            var vendas = await _vendaRepository.GetAllAsync(page, pageSize);

            // Certifique-se de que GetAllVendasResult e VendaItemDto estão definidos e acessíveis
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

        // PUT: api/Venda/{id} - UpdateVenda
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

            if (result.Success)
            {
                result.Message = "Venda atualizada com sucesso.";
                return Ok(result);
            }

            return BadRequest(result);
        }

        // DELETE: api/Venda/{id} - DeleteVenda
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(Guid id)
        {
            var vendaExistente = await _vendaRepository.GetByIdAsync(id);
            if (vendaExistente == null)
                return NotFound($"Venda com ID {id} não encontrada.");

            await _vendaRepository.DeleteAsync(vendaExistente);

            return Ok(new
            {
                Success = true,
                Message = "Venda deletada com sucesso."
            });
        }

        /// <summary>
        /// Realiza a análise de vendas para um período especificado.
        /// </summary>
        /// <param name="startDate">Data de início do período (formatoYYYY-MM-DD).</param>
        /// <param name="endDate">Data de fim do período (formatoYYYY-MM-DD).</param>
        /// <returns>Os resultados da análise de vendas.</returns>
        [HttpGet("analise")] // Define a rota para este endpoint: /api/Venda/analise
        [ProducesResponseType(typeof(GetSalesAnalysisResult), 200)] // Documenta o tipo de retorno para o Swagger
        [ProducesResponseType(400)] // Opcional: documentar erros de requisição inválida
                                    // Opcional: [Authorize(Roles = "Admin")] // Adicionar autorização se necessário
        public async Task<ActionResult<GetSalesAnalysisResult>> GetSalesAnalysis(
            [FromQuery] DateTime startDate, // Pega a data de início da query string (ex: ?startDate=2023-01-01)
            [FromQuery] DateTime endDate    // Pega a data de fim da query string (ex: ?endDate=2023-12-31)
        )
        {
            // Validação básica das datas (opcional, pode ser mais robusta em um Validator)
            if (startDate == default || endDate == default || startDate > endDate)
            {
                // Retorna BadRequest se as datas não forem válidas
                return BadRequest("As datas de início e fim do período são obrigatórias e a data de início deve ser anterior ou igual à data de fim.");
            }

            // Cria a Query com as datas recebidas
            var query = new GetSalesAnalysisQuery
            {
                // >>> CORREÇÃO PARA O ERRO DE DateTimeKind.Unspecified <<<
                // Usa .Date para garantir que a hora seja 00:00:00, e então força o Kind para Utc
                StartDate = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc),
                // Usa .Date.AddDays(1).AddSeconds(-1) para incluir o dia final inteiro (até 23:59:59), e então força o Kind para Utc
                EndDate = DateTime.SpecifyKind(endDate.Date.AddDays(1).AddSeconds(-1), DateTimeKind.Utc)
                // ------------------------------------------------------------
            };

            // Envia a Query para o MediatR e aguarda o resultado do Handler
            // O GetSalesAnalysisQueryHandler já está implementado e usará as datas ajustadas.
            var result = await _mediator.Send(query);

            // Retorna o resultado com status 200 OK
            return Ok(result);
        }
        // --------------------------------------------------------------------


    }
}