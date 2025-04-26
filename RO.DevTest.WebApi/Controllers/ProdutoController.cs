using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RO.DevTest.Application.Features.Produtos.CreateProdutoCommand;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RO.DevTest.Application.Features.Produtos; // Verifique se este using é necessário ou pode ser removido
using RO.DevTest.Application.Features.Produtos.Queries;
using RO.DevTest.Application.Features.Produtos.Commands;
using Microsoft.AspNetCore.Authorization; // <--- Mantenha esta linha
using System.Linq; // Necessário para Any()

namespace RO.DevTest.WebApi.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    // [Authorize] // <<< REMOVA o Authorize daqui, pois vamos proteger por MÉTODO >>>
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
        /// Cria um novo produto.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")] // <<< ADICIONE: Requer papel Admin para CRIAR produto >>>
        [ProducesResponseType(typeof(CreateProdutoResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CreateProdutoResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduto([FromBody] CreateProdutoCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.Sucesso && result.Id != Guid.Empty)
            {
                // Ajuste aqui para o nome correto do método GET BY ID, se necessário
                return CreatedAtAction(nameof(GetProdutoById), new { id = result.Id }, result);
            }

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
        [AllowAnonymous] // <<< ADICIONE: Permite acesso público para VER produto por ID >>>
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
        [AllowAnonymous] // <<< ADICIONE: Permite acesso público para LISTAR todos os produtos >>>
        [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProdutos()
        {
            var produtos = await _produtoRepository.GetAllAsync();
            return Ok(produtos);
        }

        /// <summary>
        /// Busca produtos com filtros e paginação.
        /// </summary>
        [HttpGet("buscar")] // Mantém a rota específica
        [AllowAnonymous] // <<< ADICIONE: Permite acesso público para BUSCAR/FILTRAR produtos (com paginação) >>>
        [ProducesResponseType(typeof(IEnumerable<Produto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProdutos([FromQuery] GetProdutosQuery request)
        {
            if (request.Page <= 0 || request.PageSize <= 0)
            {
                return BadRequest("Página e tamanho da página devem ser maiores que 0.");
            }

            try
            {
                var produtos = await _mediator.Send(request);
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
        [Authorize(Roles = "Admin")] // <<< ADICIONE: Requer papel Admin para ATUALIZAR produto >>>
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
        /// Deleta um produto existente pelo seu ID.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // <<< ADICIONE: Requer papel Admin para DELETAR produto >>>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Adicionado para consistência
        public async Task<IActionResult> DeleteProduto(Guid id)
        {
            try // Adicionado try-catch para lidar com exceções de forma mais robusta
            {
                var resultado = await _mediator.Send(new DeleteProdutoCommand(id));

                if (!resultado)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar produto: {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro inesperado ao processar a solicitação de deleção.");
            }
        }
    }
}