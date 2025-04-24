using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Produtos.CreateProdutoCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RO.DevTest.Application.Features.Produtos;
using RO.DevTest.Application.Features.Produtos.Queries;
using RO.DevTest.Application.Features.Produtos.Commands;

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IProdutoRepository _produtoRepository;

        public ProdutosController(IMediator mediator, IProdutoRepository produtoRepository)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _produtoRepository = produtoRepository ?? throw new ArgumentNullException(nameof(produtoRepository));
        }

        /// <summary>
        /// Cria um novo produto via query string.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProduto([FromBody] CreateProdutoCommand command)
        {
            var result = await _mediator.Send(command);

            // Verifique se a criação foi bem-sucedida e se o ID foi gerado corretamente
            if (result.Sucesso && result.Id != Guid.Empty) // Verificando se o ID não está vazio (Guid.Empty)
            {
                return CreatedAtAction(nameof(GetProdutoById), new { id = result.Id }, result);
            }

            // Caso contrário, retorne uma BadRequest com uma resposta de erro amigável
            var errorResponse = new
            {
                Message = "Erro ao criar o produto.",
                Details = new List<string> { result.ErrorMessage ?? "Erro desconhecido. Tente novamente mais tarde." }
            };

            return BadRequest(errorResponse);
        }

        /// <summary>
        /// Retorna um produto específico pelo seu ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GetProdutoByIdResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProdutoById(Guid id)
        {
            var result = await _mediator.Send(new GetProdutoByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Retorna todos os produtos cadastrados.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProdutos()
        {
            var produtos = await _produtoRepository.GetAllAsync();
            return Ok(produtos);
        }

        [HttpGet("buscar")]
        [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProdutos([FromQuery] GetProdutosQuery request)
        {
            // Validando os parâmetros de página e tamanho da página
            if (request.Page <= 0 || request.PageSize <= 0)
            {
                return BadRequest("Página e tamanho da página devem ser maiores que 0.");
            }

            try
            {
                // Aqui, o MediatR enviará a consulta para o handler adequado
                var produtos = await _mediator.Send(request);

                // Retorna os produtos encontrados
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(UpdateProdutoResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduto(Guid id, [FromBody] UpdateProdutoCommand command)
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
                Console.WriteLine($"Erro ao atualizar produto: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro inesperado ao processar a solicitação.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(Guid id)
        {
            var resultado = await _mediator.Send(new DeleteProdutoCommand(id));

            if (!resultado)
                return NotFound();

            return NoContent();
        }


    }
}
