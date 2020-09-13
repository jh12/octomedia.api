using System;
using OctoMedia.Api.DataAccess.Mssql.Models;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Media.Meta;

namespace OctoMedia.Api.DataAccess.Mssql.Mappers
{
    public static class MediaMapper
    {
        public static KeyedMedia Map(DBMedia media)
        {
            Author? author = media.AuthorUsername != null ? new Author(media.AuthorUsername) : null;
            Dimension? dimension = media.Height.HasValue && media.Width.HasValue ? new Dimension(media.Height.Value, media.Width.Value) : null;
            FileType fileType = new FileType(media.FileTypeExtension, media.FileTypeClass);

            return new KeyedMedia(
                media.Id,
                media.Title,
                media.Description,
                author,
                dimension,
                media.SourceId,
                new Uri(media.ImageUri), 
                fileType,
                media.Mature,
                media.Approved,
                media.Deleted
                );
        }
    }
}