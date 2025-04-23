using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Clientes.CreateClienteCommand;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="command">Dados para criação do cliente.</param>
        /// <returns>Resultado da criação do cliente.</returns>
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
        /// <param name="id">O ID do cliente a ser buscado.</param>
        /// <returns>Os dados do cliente.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)] // Alterado para object
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetClienteById(Guid id)
        {
            // TODO: Implementar a lógica para buscar um cliente por ID
            return NotFound(); // Temporariamente retorna NotFound
        }

        /// <summary>
        /// Lista todos os clientes.
        /// </summary>
        /// <returns>Uma lista de todos os clientes.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)] // Alterado para IEnumerable<object>
        public IActionResult GetAllClientes()
        {
            // TODO: Implementar a lógica para listar todos os clientes
            return Ok(new List<string>()); // Temporariamente retorna uma lista vazia
        }

        /// <summary>
        /// Atualiza um cliente existente.
        /// </summary>
        /// <param name="id">O ID do cliente a ser atualizado.</param>
        /// <param name="command">Dados para atualização do cliente.</param>
        /// <returns>Resultado da atualização.</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateCliente(Guid id, [FromBody] object command) // Alterado para object
        {
            // TODO: Implementar a lógica para atualizar um cliente
            return NotFound(); // Temporariamente retorna NotFound
        }

        /// <summary>
        /// Remove um cliente pelo seu ID.
        /// </summary>
        /// <param name="id">O ID do cliente a ser removido.</param>
        /// <returns>Resultado da remoção.</returns>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteCliente(Guid id)
        {
            // TODO: Implementar a lógica para remover um cliente
            return NotFound(); // Temporariamente retorna NotFound
        }
    }
}