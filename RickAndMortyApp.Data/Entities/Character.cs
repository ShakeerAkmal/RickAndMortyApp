using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RickAndMortyApp.Data.Entities
{
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Type { get; set; }
        public string Gender { get; set; } = null!;
        public string Image { get; set; } = null!;
        public DateTime Created { get; set; }
        public int? OriginId { get; set; }
        public int? CurrentLocationId { get; set; }
        
        public Location? Origin { get; set; } = null!;
        public Location? Location { get; set; } = null!;
        public ICollection<CharacterEpisode> CharacterEpisodes { get; set; } = new List<CharacterEpisode>();

    }

}
