using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Clientes.CreateClienteCommand;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RO.DevTest.Application.Features.Clientes.UpdateClienteCommand;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; // Ensure this is included for ILogger

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IClienteRepository _clienteRepository;
        private readonly IVendaRepository _vendaRepository;
        private readonly ILogger<ClientesController> _logger; // Declare _logger field

        public ClientesController(
            IMediator mediator,
            IClienteRepository clienteRepository,
            IVendaRepository vendaRepository,
            ILogger<ClientesController> logger) // Inject ILogger
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _vendaRepository = vendaRepository ?? throw new ArgumentNullException(nameof(vendaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Assign to _logger
        }

        /// <summary>
        /// Cria um novo cliente (via parâmetros na query string).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromQuery] CreateClienteCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Success && result.ClienteId != Guid.Empty)
            {
                return CreatedAtAction(nameof(GetClienteById), new { id = result.ClienteId }, result);
            }

            return BadRequest(result.ErrorMessage);
        }

        /// <summary>
        /// Obtém um cliente pelo seu ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Cliente), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClienteById(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        /// <summary>
        /// Lista todos os clientes.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Cliente>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllClientes()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Atualiza um cliente existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UpdateClienteResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCliente(Guid id, [FromBody] UpdateClienteCommand command)
        {
            if (id != command.Id)
            {
                _logger.LogWarning("ID mismatch: Route ID {RouteId} does not match command ID {CommandId}", id, command.Id);
                return BadRequest("O ID na rota não corresponde ao ID no corpo da requisição.");
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating client with ID: {ClienteId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro inesperado ao processar a solicitação.");
            }
        }

        /// <summary>
        /// Remove um cliente pelo seu ID.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete client with ID: {ClienteId}", id);

                // Find the client
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Client not found: {ClienteId}", id);
                    return NotFound(new { Message = "Client not found" });
                }

                // Check for related Vendas records
                var hasVendas = await _vendaRepository.AnyAsync(v => v.ClienteId == id);
                if (hasVendas)
                {
                    _logger.LogWarning("Cannot delete client {ClienteId} because they have associated sales", id);
                    return BadRequest(new { Message = "Cannot delete client because they have associated sales" });
                }

                // Delete the client
                await _clienteRepository.DeleteAsync(id);
                _logger.LogInformation("Client deleted successfully: {ClienteId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting client with ID: {ClienteId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Internal server error while deleting client" });
            }
        }
    }
}