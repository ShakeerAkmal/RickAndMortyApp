using System;
using System.Collections.Generic;

namespace RickAndMorty.Services.Dtos
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Type { get; set; }
        public string Gender { get; set; } = null!;
        public string Image { get; set; } = null!;
        public DateTime Created { get; set; }
        public int OriginId { get; set; }
        public int CurrentLocationId { get; set; }
        
        public LocationDto Origin { get; set; } = null!;
        public LocationDto Location { get; set; } = null!;
        public ICollection<CharacterEpisode> CharacterEpisodes { get; set; } = new List<CharacterEpisode>();

    }

}
