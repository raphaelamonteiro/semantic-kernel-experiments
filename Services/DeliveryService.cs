using RestSharp;

public class DeliveryApiService
{
    private readonly RestClient _client;

    public DeliveryApiService()
    {
        _client = new RestClient("http://localhost:5256");
    }

    public async Task<string> GetProdutosAsync()
    {
        var request = new RestRequest("/api/produtos", Method.Get);

        var response = await _client.ExecuteAsync(request);

        return response.Content ?? "";
    }
}