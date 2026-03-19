using Microsoft.SemanticKernel;
using chat_with_api.State;
using chat_with_api.Services;
using System.Text;

namespace chat_with_api.Plugins;

public class DeliveryPlugin
{
    private readonly DeliveryApiService _service;
    private readonly PedidoState _state;

    public DeliveryPlugin(DeliveryApiService service, PedidoState state)
    {
        _service = service;
        _state = state;
    }

    // TELEFONE
    [KernelFunction]
    public string InformarTelefone(string telefone)
    {
        _state.Telefone = telefone;
        return $"Telefone {telefone} registrado com sucesso.";
    }

    // BUSCAR PRODUTOS
    [KernelFunction]
    public async Task<string> BuscarProdutos(string nome)
    {
        var produtos = await _service.BuscarProdutosAsync(nome);

        if (produtos == null || produtos.Count == 0)
            return "Nenhum produto encontrado.";

        var sb = new StringBuilder();

        foreach (var p in produtos)
        {
            sb.AppendLine($"{p.Descricao} - R$ {p.Preco}");
        }

        return sb.ToString();
    }

    // ADICIONAR ITEM
    [KernelFunction]
    public async Task<string> AdicionarItemPedido(string nome, int quantidade)
    {
        // trava: precisa de telefone primeiro
        if (string.IsNullOrEmpty(_state.Telefone))
            return "Antes de fazer o pedido, pode me informar seu telefone?";

        var produtos = await _service.BuscarProdutosAsync(nome);

        if (produtos == null || produtos.Count == 0)
            return $"Não encontrei '{nome}' no cardápio. Quer ver algumas opções?";

        var produto = produtos.First();

        _state.Itens.Add(new ItemPedido
        {
            Nome = produto.Descricao,
            Quantidade = quantidade,
            Preco = produto.Preco
        });

        return $"{quantidade}x {produto.Descricao} adicionado ao pedido por R$ {produto.Preco}.";
    }

    // VER PEDIDO
    [KernelFunction]
    public string VerPedido()
    {
        if (!_state.Itens.Any())
            return "Seu pedido está vazio.";

        var sb = new StringBuilder();
        decimal total = 0;

        foreach (var item in _state.Itens)
        {
            var subtotal = item.Preco * item.Quantidade;
            total += subtotal;

            sb.AppendLine($"{item.Quantidade}x {item.Nome} - R$ {subtotal}");
        }

        sb.AppendLine($"\nTotal: R$ {total}");

        return sb.ToString();
    }
}