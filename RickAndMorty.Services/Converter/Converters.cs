using RickAndMorty.Services.Dtos;
using RickAndMortyApp.Data.Entities;

namespace RickAndMorty.Services.Converter
{

    public static class Converters
    {
        public static Character ToEntity(CharacterDto dto)
        {
            return new Character
            {
                Id = dto.Id,
                Name = dto.Name,
                Status = dto.Status,
                Species = dto.Species,
                Type = dto.Type,
                Gender = dto.Gender,
                Image = dto.Image,
                Created = dto.Created,
                CurrentLocationId = dto.CurrentLocationId,
                OriginId = dto.OriginId

            };
        }

        public static Location ToEntity(LocationDto dto)
        {
            return new Location
            {
                Id = dto.Id,
                Name = dto.Name,
                Type = dto.Type,
                Dimension = dto.Dimension,
                Url = dto.Url,
                Created = dto.Created
            };
        }
        public static Episode ToEntity(EpisodeDto dto)
        {
            return new Episode
            {
                Id = dto.Id,
                Name = dto.Name,
                AirDate = dto.AirDate,
                EpisodeCode = dto.Episode,
                Url = dto.Url,
                Created = dto.Created
            };
        }

        public static CharacterDto ToDto(Character entity)
        {
            return new CharacterDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Status = entity.Status,
                Species = entity.Species,
                Type = entity.Type,
                Gender = entity.Gender,
                Image = entity.Image,
                Created = entity.Created,
                OriginId = entity.OriginId,
                CurrentLocationId = entity.CurrentLocationId,
                CharacterEpisodes = entity.CharacterEpisodes?.Select(ce => new CharacterEpisodeDto
                {
                    CharacterId = ce.CharacterId,
                    EpisodeId = ce.EpisodeId,
                    Episode = ce.Episode != null ? new EpisodeDto
                    {
                        Id = ce.Episode.Id,
                        Name = ce.Episode.Name,
                        AirDate = ce.Episode.AirDate,
                        Episode = ce.Episode.EpisodeCode,
                        Url = ce.Episode.Url,
                        Created = ce.Episode.Created
                    } : null

                }).ToList() ?? new List<CharacterEpisodeDto>(),
                Origin = entity.Origin != null ? new LocationDto
                {
                    Id = entity.Origin.Id,
                    Name = entity.Origin.Name,
                    Type = entity.Origin.Type,
                    Dimension = entity.Origin.Dimension,
                    Url = entity.Origin.Url,
                    Created = entity.Origin.Created
                } : null,
                Location = entity.Location != null ? new LocationDto
                {
                    Id = entity.Location.Id,
                    Name = entity.Location.Name,
                    Type = entity.Location.Type,
                    Dimension = entity.Location.Dimension,
                    Url = entity.Location.Url,
                    Created = entity.Location.Created
                } : null
            };
        }
    }
}
