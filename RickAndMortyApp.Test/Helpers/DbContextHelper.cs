using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using RickAndMortyApp.Data;

namespace RickAndMortyApp.Test.Helpers
{
    public static class DbContextHelper
    {
        public static RickAndMortyContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<RickAndMortyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new RickAndMortyContext(options);
        }
    }
}
