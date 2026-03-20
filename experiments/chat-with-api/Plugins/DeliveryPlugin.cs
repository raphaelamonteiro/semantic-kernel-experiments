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
        if (!string.IsNullOrEmpty(_state.Telefone))
            return $"Já tenho seu telefone como {_state.Telefone}. Deseja alterar?";

        _state.Telefone = telefone;
        _state.EtapaAtual = EtapaPedido.EscolhendoItens;

        return "Telefone registrado. O que você deseja pedir?";
    }

    // BUSCAR PRODUTOS[KernelFunction]
    public async Task<string> BuscarProdutos(string? nome)
    {
        // 🔥 evita busca pesada sem filtro
        if (string.IsNullOrWhiteSpace(nome))
            return "Por favor, informe o nome de um produto para buscar.";

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


    [KernelFunction]
    public async Task<string> ListarProdutos()
    {
        if (string.IsNullOrEmpty(_state.Telefone))
        {
            _state.EtapaAtual = EtapaPedido.AguardandoTelefone;
            return "Antes de ver o cardápio, pode me informar seu telefone?";
        }

        var produtos = await _service.BuscarProdutosAsync();

        if (produtos == null || produtos.Count == 0)
            return "Nenhum produto disponível.";

        var sb = new StringBuilder();

        foreach (var p in produtos.Take(10))
        {
            sb.AppendLine($"{p.Descricao} - R$ {p.Preco}");
        }

        return sb.ToString();
    }

    // ADICIONAR ITEM
    [KernelFunction]
    public async Task<string> AdicionarItemPedido(string nome, int quantidade)
    {
        if (string.IsNullOrEmpty(_state.Telefone))
        {
            _state.EtapaAtual = EtapaPedido.AguardandoTelefone;
            return "Antes de fazer o pedido, pode me informar seu telefone?";
        }

        var produtos = await _service.BuscarProdutosAsync(nome);

        if (produtos == null || produtos.Count == 0)
            return $"Não encontrei '{nome}' no cardápio.";

        var produto = produtos.First();

        var itemExistente = _state.Itens
            .FirstOrDefault(i => i.Nome == produto.Descricao);

        if (itemExistente != null)
        {
            itemExistente.Quantidade += quantidade;
        }
        else
        {
            _state.Itens.Add(new ItemPedido
            {
                Nome = produto.Descricao,
                Quantidade = quantidade,
                Preco = produto.Preco
            });
        }

        _state.EtapaAtual = EtapaPedido.AguardandoEndereco;

        return $"{quantidade}x {produto.Descricao} adicionado. Deseja informar o endereço?";
    }

    [KernelFunction]
    public string InformarEndereco(string endereco)
    {
        if (!_state.Itens.Any())
            return "Adicione itens antes de informar o endereço.";

        _state.Endereco = endereco;
        _state.EtapaAtual = EtapaPedido.AguardandoPagamento;

        return $"Endereço registrado. Qual a forma de pagamento?";
    }

    [KernelFunction]
    public string InformarPagamento(string formaPagamento)
    {
        if (string.IsNullOrEmpty(_state.Endereco))
            return "Preciso do endereço antes da forma de pagamento.";

        _state.FormaPagamento = formaPagamento;
        _state.EtapaAtual = EtapaPedido.ConfirmacaoFinal;

        return $"Pagamento '{formaPagamento}' registrado. Deseja finalizar o pedido?";
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

    [KernelFunction]
    public string FinalizarPedido()
    {
        if (string.IsNullOrEmpty(_state.Telefone))
            return "Falta telefone.";

        if (!_state.Itens.Any())
            return "Seu pedido está vazio.";

        if (string.IsNullOrEmpty(_state.Endereco))
            return "Falta endereço.";

        if (string.IsNullOrEmpty(_state.FormaPagamento))
            return "Falta forma de pagamento.";

        _state.PedidoFinalizado = true;
        _state.EtapaAtual = EtapaPedido.Finalizado;

        return "Pedido finalizado com sucesso! 🚚";
    }

    //LIMPAR PEDIDO
    [KernelFunction]
    public string LimparPedido()
    {
        _state.Itens.Clear();
        return "Pedido limpo com sucesso.";
    }
}