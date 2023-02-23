using DevEnglishTutor.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DevEnglishTutor.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnglishTutorController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public EnglishTutorController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string text, [FromServices] IConfiguration configuration)
    {
        var token = configuration.GetValue<string>("ChatPgtSecretKey");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var model = new ChatGptInputModel(text);
        var requestBody = JsonSerializer.Serialize(model);
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openia.com/v1/completions", content);

        var result = await response.Content.ReadFromJsonAsync<ChatGptViewModel>();
        var promptResponse = result.choices.First();

        return Ok(promptResponse.text.Replace("\n", "").Replace("\t", ""));
    }
}
