namespace RickAndMorty.Services.Dtos
{
    public class LocationDto
    {
        public int Id { get; set; }  
        public string Name { get; set; } = null!;
        public string Type { get; set; } = "";
        public string Dimension { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime Created { get; set; }

        public List<string> Residents { get; set; } 
        public ICollection<CharacterDto> OriginCharacters { get; set; } = new List<CharacterDto>();
    }
}
