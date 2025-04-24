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
        /// Obtém um produto pelo seu ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProdutoById(Guid id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
                return NotFound();

            return Ok(produto);
        }

        /// <summary>
        /// Lista todos os produtos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProdutos()
        {
            var produtos = await _produtoRepository.GetAllAsync();
            return Ok(produtos);
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

        /// <summary>
        /// Remove um produto pelo seu ID.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduto(Guid id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
                return NotFound();

            await _produtoRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
