namespace RickAndMorty.Services.Dtos
{
    public class CreateCharacterDto
    {
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Type { get; set; }
        public string Gender { get; set; } = null!;
        public string Image { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int? OriginId { get; set; }
        public int? CurrentLocationId { get; set; }
        public List<int> EpisodeIds { get; set; }
    }

}
