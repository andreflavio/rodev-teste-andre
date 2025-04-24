using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Clientes.CreateClienteCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RO.DevTest.Application.Features.Clientes.UpdateClienteCommand;

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IClienteRepository _clienteRepository;

        public ClientesController(IMediator mediator, IClienteRepository clienteRepository)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
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

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UpdateClienteResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCliente(Guid id, [FromBody] UpdateClienteCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("O ID na rota não corresponde ao ID no corpo da requisição.");
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar cliente: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro inesperado ao processar a solicitação.");
            }
        }


        /// <summary>
        /// Remove um cliente pelo seu ID.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCliente(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();

            await _clienteRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
