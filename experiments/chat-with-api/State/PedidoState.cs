namespace chat_with_api.State;

public class PedidoState
{
    public string? Telefone { get; set; }
    public string? ClienteNome { get; set; }
    public string? Endereco { get; set; }

    public List<ItemPedido> Itens { get; set; } = new();

    public bool PedidoConfirmado { get; set; } = false;
}

public class ItemPedido
{
    public string Nome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }
}