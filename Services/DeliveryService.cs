using RestSharp;
using System.Text.Json;

public class DeliveryApiService
{
    private readonly RestClient _client;

    public DeliveryApiService()
    {
        _client = new RestClient("http://localhost:5256");

        var token = Environment.GetEnvironmentVariable("API_TOKEN");

        if (string.IsNullOrEmpty(token))
            throw new Exception("API_TOKEN não configurado");

        _client.AddDefaultHeader("Authorization", $"Bearer {token}");
    }
    public async Task<List<ProdutoDto>?> BuscarProdutosAsync(
        string? nome = null,
        int operador = 1)
    {
        var request = new RestRequest("/Produto/Consultar", Method.Post);

        var body = new ConsultaDto
        {
            RegistrosPorPagina = 10,
            NumeroPagina = 1,
            ListPesquisaDto = string.IsNullOrEmpty(nome)
                ? new List<PesquisaDto>()
                : new List<PesquisaDto>
                {
                new PesquisaDto
                {
                    AtributoPesquisa = "descricao",
                    Operador = operador,
                    ValorPesquisa = nome
                }
                }
        };

        request.AddJsonBody(body);

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful || string.IsNullOrEmpty(response.Content))
        {
            Console.WriteLine($"Erro API: {response.StatusCode}");
            Console.WriteLine(response.Content);
            return null;
        }

        return JsonSerializer.Deserialize<List<ProdutoDto>>(
            response.Content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
    }
}