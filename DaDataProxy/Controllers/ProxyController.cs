using System;
using System.Net;
using System.Threading.Tasks;
using DaDataProxy.Models.Address;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("/proxy")]
public class ProxyController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly DataModel _dataModel;
    private readonly IConfiguration _configuration;


    public ProxyController(IConfiguration configuration)
    {
        _configuration = configuration;
        _dataModel = new DataModel();
        _httpClient = new HttpClient();
    }

    [HttpGet]
    [Route("/proxy/getaddress")]
    public async Task<IActionResult> GetAddressSuggestions(string query)
    {
        // Добавляем авторизационный заголовок
        string _apiKey = _configuration.GetSection("ApiSettings:ApiKey").Value;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {_apiKey}");
        string authorizationHeader = Request.Headers["Authorization"];

        // ENV переменная
        string _proxyKey = _configuration.GetSection("Proxy-Authorization:ProxyKey").Value;
        _httpClient.DefaultRequestHeaders.Add("Proxy-Authorization", $"Token {_proxyKey}");
        string authProxy = Request.Headers["Proxy-Authorization"];


        string apiUrl = $"https://suggestions.dadata.ru/suggestions/api/4_1/rs/suggest/address?query={query}";
        HttpRequestMessage targetRequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);


        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }

        if (authorizationHeader != _proxyKey)
        {
            return StatusCode((int)HttpStatusCode.Unauthorized);
        }

        HttpResponseMessage targetResponse = await _httpClient.SendAsync(targetRequest);

        if (targetResponse.IsSuccessStatusCode)
        {
            string responseContent = await targetResponse.Content.ReadAsStringAsync();
            DataModel dataModel = JsonConvert.DeserializeObject<DataModel>(responseContent);

            List<string> streetsList = dataModel.Suggestions.Select(s => s.Value).ToList();
            return Ok(streetsList);
        }
        else
        {
            return StatusCode((int)targetResponse.StatusCode);
        }
    }
}
