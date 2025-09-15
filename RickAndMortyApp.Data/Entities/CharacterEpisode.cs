namespace RickAndMortyApp.Data.Entities
{
    public class CharacterEpisode
    {
        public int CharacterId { get; set; }
        public int EpisodeId { get; set; }
        public Character Character { get; set; } = null!;
        public Episode Episode { get; set; } = null!;
    }

}
