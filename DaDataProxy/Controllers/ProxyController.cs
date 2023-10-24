using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DaDataProxy.Models.Address;
using DaDataProxy.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class ProxyController : ControllerBase
{
    private readonly ProxySettings _proxySettings;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProxyController(IOptions<ProxySettings> proxySettings, IHttpClientFactory httpClientFactory)
    {
        _proxySettings = proxySettings.Value;
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("/proxy/getaddress")]
    public async Task<IActionResult> GetAddressSuggestionsAsync(string query)
    {
        var httpFactory = _httpClientFactory.CreateClient();
        string authorizationHeader = Request.Headers["Authorization"];

        if (authorizationHeader != _proxySettings.ProxyKey)
        {
            return StatusCode((int)HttpStatusCode.Unauthorized);
        } 
        else if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }
        else
        {
            httpFactory.DefaultRequestHeaders.Add("Authorization", $"Token {_proxySettings.ApiKey}");

            using var targetRequest = new HttpRequestMessage(HttpMethod.Get, _proxySettings.BaseUrl + $"?query={query}");
            using var targetResponse = await httpFactory.SendAsync(targetRequest);
            string responseContent = await targetResponse.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<DataModel>(responseContent);
            List<string> streetsList = responseObject.Suggestions.Select(x => x.Value).ToList();
            return Ok(streetsList);
        }
    }
}