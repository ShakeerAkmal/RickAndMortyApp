using System.Text.Json.Serialization;

namespace RickAndMorty.Services.Dtos
{
    public class EpisodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        [JsonPropertyName("air_date")]
        public string AirDate { get; set; } = "";
        public string Episode { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime Created { get; set; }
    }
}
