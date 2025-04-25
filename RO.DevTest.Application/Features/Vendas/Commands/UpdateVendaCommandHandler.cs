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
    public class UpdateVendaCommandHandler : IRequestHandler<UpdateVendaCommand, UpdateVendaResult>
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProdutoRepository _produtoRepository;

        public UpdateVendaCommandHandler(
            IVendaRepository vendaRepository,
            IClienteRepository clienteRepository,
            IProdutoRepository produtoRepository)
        {
            _vendaRepository = vendaRepository ?? throw new ArgumentNullException(nameof(vendaRepository));
            _clienteRepository = clienteRepository ?? throw new ArgumentNullException(nameof(clienteRepository));
            _produtoRepository = produtoRepository ?? throw new ArgumentNullException(nameof(produtoRepository));
        }

        public async Task<UpdateVendaResult> Handle(UpdateVendaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar venda
                var venda = await _vendaRepository.GetByIdAsync(request.VendaId);
                if (venda == null)
                {
                    return new UpdateVendaResult { Success = false, Message = "Venda não encontrada." };
                }

                // Validar cliente
                var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
                if (cliente == null)
                {
                    return new UpdateVendaResult { Success = false, Message = "Cliente não encontrado." };
                }

                // Validar itens
                if (request.Itens == null || !request.Itens.Any())
                {
                    return new UpdateVendaResult { Success = false, Message = "A venda deve conter pelo menos um item." };
                }

                var itensVenda = new List<ItemVenda>();
                foreach (var item in request.Itens)
                {
                    // Validar produto
                    var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);
                    if (produto == null)
                    {
                        return new UpdateVendaResult { Success = false, Message = $"Produto com ID {item.ProdutoId} não encontrado." };
                    }

                    // Validar quantidade
                    if (item.Quantidade <= 0)
                    {
                        return new UpdateVendaResult { Success = false, Message = $"Quantidade inválida para o produto {produto.Nome}." };
                    }

                    // Calcular quantidade adicional necessária
                    var itemAtual = venda.Itens.FirstOrDefault(i => i.ProdutoId == item.ProdutoId);
                    int quantidadeAtual = itemAtual?.Quantidade ?? 0;
                    int quantidadeAdicional = item.Quantidade - quantidadeAtual;

                    // Validar estoque
                    if (quantidadeAdicional > 0 && produto.Estoque < quantidadeAdicional)
                    {
                        return new UpdateVendaResult { Success = false, Message = $"Estoque insuficiente para o produto {produto.Nome}. Disponível: {produto.Estoque}, Solicitado: {quantidadeAdicional}." };
                    }

                    // Atualizar estoque
                    if (quantidadeAdicional != 0)
                    {
                        produto.Estoque -= quantidadeAdicional;
                        await _produtoRepository.UpdateAsync(produto);
                    }

                    itensVenda.Add(new ItemVenda
                    {
                        ProdutoId = item.ProdutoId,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.PrecoUnitario
                    });
                }

                // Atualizar venda
                venda.ClienteId = request.ClienteId;
                venda.DataVenda = request.DataVenda;
                venda.Itens = itensVenda;

                // Calcular total para validação
                var totalCalculado = itensVenda.Sum(i => i.Quantidade * i.PrecoUnitario);
                if (Math.Abs(totalCalculado - request.ValorTotal) > 0.01m)
                {
                    return new UpdateVendaResult { Success = false, Message = $"ValorTotal informado ({request.ValorTotal}) não corresponde ao calculado ({totalCalculado})." };
                }

                // Persistir a venda
                // Como UpdateAsync não existe, o EF rastreia as alterações automaticamente
                // Não é necessário chamar um método explícito de atualização

                return new UpdateVendaResult
                {
                    Success = true,
                    VendaId = venda.Id,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO AO ATUALIZAR VENDA: {ex.Message}");
                return new UpdateVendaResult { Success = false, Message = "Erro ao atualizar a venda." };
            }
        }
    }
}