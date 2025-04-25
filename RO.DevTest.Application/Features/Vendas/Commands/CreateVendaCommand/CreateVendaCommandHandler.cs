using MediatR;
using RO.DevTest.Application.Contracts.Persistence.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Vendas.Commands
{
    public class CreateVendaCommandHandler : IRequestHandler<CreateVendaCommand, CreateVendaResult>
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProdutoRepository _produtoRepository;

        public CreateVendaCommandHandler(
            IVendaRepository vendaRepository,
            IClienteRepository clienteRepository,
            IProdutoRepository produtoRepository)
        {
            _vendaRepository = vendaRepository ?? throw new ArgumentNullException(nameof(vendaRepository));
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _produtoRepository = produtoRepository ?? throw new ArgumentNullException(nameof(produtoRepository));
        }

        public async Task<CreateVendaResult> Handle(CreateVendaCommand request, CancellationToken cancellationToken)
        {
            // Validar cliente
            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente == null)
            {
                return new CreateVendaResult { Success = false, Message = "Cliente não encontrado." };
            }

            // Validar itens
            if (request.Itens == null || !request.Itens.Any())
            {
                return new CreateVendaResult { Success = false, Message = "A venda deve conter pelo menos um item." };
            }

            var itensVenda = new List<ItemVenda>();
            foreach (var item in request.Itens)
            {
                // Validar produto
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);
                if (produto == null)
                {
                    return new CreateVendaResult { Success = false, Message = $"Produto com ID {item.ProdutoId} não encontrado." };
                }

                // Validar quantidade
                if (item.Quantidade <= 0)
                {
                    return new CreateVendaResult { Success = false, Message = $"Quantidade inválida para o produto {produto.Nome}." };
                }

                // Validar estoque
                if (produto.Estoque < item.Quantidade)
                {
                    return new CreateVendaResult { Success = false, Message = $"Estoque insuficiente para o produto {produto.Nome}. Disponível: {produto.Estoque}, Solicitado: {item.Quantidade}." };
                }

                // Atualizar estoque
                produto.Estoque -= item.Quantidade;
                await _produtoRepository.UpdateAsync(produto);

                itensVenda.Add(new ItemVenda
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario // Preço unitário vem do comando
                });
            }

            // Criar a venda
            var venda = new Venda
            {
                ClienteId = request.ClienteId,
                DataVenda = request.DataVenda,
                Itens = itensVenda
            };

            try
            {
                // Persistir a venda
                await _vendaRepository.CreateAsync(venda); // Ajustado para usar CreateAsync sem CancellationToken

                // Retornar o resultado
                return new CreateVendaResult
                {
                    Success = true,
                    VendaId = venda.Id,
                    Message = string.Empty
                };
            }
            catch (Exception)
            {
                // Log the exception for more detailed error information in a real application
                return new CreateVendaResult { Success = false, Message = "Erro ao criar a venda." };
            }
        }
    }
}