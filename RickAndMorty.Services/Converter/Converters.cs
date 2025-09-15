using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


    }
}
