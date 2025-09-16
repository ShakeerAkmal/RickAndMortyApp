using Microsoft.EntityFrameworkCore;
using RickAndMortyApp.Data;

namespace RickAndMortyApp.Test.Helpers
{
    public static class DbContextHelper
    {
        public static RickAndMortyContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<RickAndMortyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new RickAndMortyContext(options);
        }
    }
}
