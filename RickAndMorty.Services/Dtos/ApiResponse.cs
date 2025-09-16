using System.Text.Json.Serialization;

namespace RickAndMorty.Services.Dtos
{
    public class ApiResponse<T>
    {
        public ApiInfo Info { get; set; } = default!;
        public List<T> Results { get; set; } = new();
    }

    public class ApiInfo
    {
        public int Count { get; set; }
        public int Pages { get; set; }
        public string? Next { get; set; }
        public string? Prev { get; set; }
    }

}
