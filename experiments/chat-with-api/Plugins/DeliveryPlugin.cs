namespace chat_with_api.Plugins;

using Microsoft.SemanticKernel;
using System.Text;
using chat_with_api.Services;
using chat_with_api.DTO;

public class DeliveryPlugin
{
    private readonly DeliveryApiService _service;

    public DeliveryPlugin()
    {
        _service = new DeliveryApiService();
    }

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
}