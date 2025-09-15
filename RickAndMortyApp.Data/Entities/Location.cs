namespace RickAndMortyApp.Data.Entities
{
    public class Location
    {
        public int Id { get; set; }  
        public string Name { get; set; } = null!;
        public string Type { get; set; } = "";
        public string Dimension { get; set; } = "";
        public DateTime Created { get; set; }

        public ICollection<Character> Residents { get; set; } = new List<Character>();
        public ICollection<Character> OriginCharacters { get; set; } = new List<Character>();
    }
}
