namespace RickAndMorty.Services.Interfaces
{
    public interface ILocationService
    {
        Task FetchAndSaveLocationsAsync();
        Task DeleteAllData();

    }

}
