using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RickAndMortyApp.Data.Entities
{
    public class Episode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string AirDate { get; set; } = "";
        public string EpisodeCode { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime Created { get; set; }
        public ICollection<CharacterEpisode> CharacterEpisodes { get; set; } = new List<CharacterEpisode>();
    }
}
