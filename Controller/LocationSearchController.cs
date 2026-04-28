using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MobileApi.Models;

namespace MobileApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationSearchController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LocationSearchController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string q = "lost item", CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new { message = "Query cannot be empty." });
            }

            var client = _httpClientFactory.CreateClient("Nominatim");
            var requestUri = $"search?q={Uri.EscapeDataString(q)}&format=json";

            try
            {
                using var response = await client.GetAsync(requestUri, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "Location search is temporarily unavailable."
                    });
                }

                await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var results = await JsonSerializer.DeserializeAsync<List<LocationSearchResult>>(
                    responseStream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken);

                return Ok(results ?? new List<LocationSearchResult>());
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    message = "Unable to reach the location provider right now."
                });
            }
        }
    }
}
