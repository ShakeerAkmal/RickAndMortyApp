namespace RickAndMortyApp.Data.Entities
{
    public class Episode
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string AirDate { get; set; } = "";
        public string EpisodeCode { get; set; } = "";
        public DateTime Created { get; set; }
        public ICollection<CharacterEpisode> CharacterEpisodes { get; set; } = new List<CharacterEpisode>();
    }
}
