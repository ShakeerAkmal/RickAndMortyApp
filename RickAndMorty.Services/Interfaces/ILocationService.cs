namespace RickAndMorty.Services.Interfaces
{
    public interface ILocationService
    {
        Task DeleteAllData(CancellationToken cancellationToken = default);
        Task FetchAndSaveLocationsAsync(CancellationToken cancellationToken = default);

    }

}
