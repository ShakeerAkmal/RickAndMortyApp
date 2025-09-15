namespace RickAndMorty.Services.Dtos
{
    public class CharacterEpisode
    {
        public int CharacterId { get; set; }
        public int EpisodeId { get; set; }
        public CharacterDto Character { get; set; } = null!;
        public EpisodeDto Episode { get; set; } = null!;
    }

}
