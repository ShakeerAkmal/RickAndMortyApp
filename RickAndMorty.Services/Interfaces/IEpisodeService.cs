namespace RickAndMorty.Services.Interfaces
{
    public interface IEpisodeService
    {
        Task FetchAndSaveEpisodesAsync(CancellationToken cancellationToken = default);
    }

}
