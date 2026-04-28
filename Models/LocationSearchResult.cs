using System.Text.Json.Serialization;

namespace MobileApi.Models
{
    public class LocationSearchResult
    {
        [JsonPropertyName("place_id")]
        public long PlaceId { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = "";

        [JsonPropertyName("lat")]
        public string Latitude { get; set; } = "";

        [JsonPropertyName("lon")]
        public string Longitude { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("class")]
        public string Category { get; set; } = "";
    }
}
