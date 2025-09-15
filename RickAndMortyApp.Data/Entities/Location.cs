using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RickAndMortyApp.Data.Entities
{
    public class Location
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }  
        public string Name { get; set; } = null!;
        public string Type { get; set; } = "";
        public string Url { get; set; } = "";
        public string Dimension { get; set; } = "";
        public DateTime Created { get; set; }

        public ICollection<Character> Residents { get; set; } = new List<Character>();
        public ICollection<Character> OriginCharacters { get; set; } = new List<Character>();
    }
}
