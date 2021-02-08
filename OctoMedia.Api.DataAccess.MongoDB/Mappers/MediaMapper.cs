using OctoMedia.Api.DataAccess.MongoDB.Models;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Media.Meta;

namespace OctoMedia.Api.DataAccess.MongoDB.Mappers
{
    internal static class MediaMapper
    {
        public static KeyedMedia Map(MongoMedia media)
        {
            Author? author = media.Author != null ? new Author(media.Author.Username) : null;
            Dimension? dimension = media.Dimensions != null ? new Dimension(media.Dimensions.Height, media.Dimensions.Width) : null;
            FileType fileType = new FileType(media.FileType.Extension, media.FileType.Class);

            return new KeyedMedia(
                media.Id,
                media.Title,
                media.Description,
                author,
                dimension,
                media.SourceId,
                media.ImageUri,
                fileType,
                media.Mature,
                media.Approved,
                media.Deleted);
        }

        public static MongoMedia Map(KeyedMedia media)
        {
            MongoMedia mongoMedia = Map(media);

            mongoMedia.Id = mongoMedia.Id;

            return mongoMedia;
        }

        public static MongoMedia Map(Media media)
        {
            MongoMediaAuthor? author = media.Author != null ? new MongoMediaAuthor {Username = media.Author.Username} : null;
            MongoMediaDimensions? dimensions = media.Dimension != null ? new MongoMediaDimensions { Height = media.Dimension.Height, Width = media.Dimension.Width} : null;
            MongoMediaFileType fileType = new() {Class = media.FileType.FileClass, Extension = media.FileType.Extension};

            return new MongoMedia
            {
                Title = media.Title,
                Description = media.Description,
                Author = author,
                Approved = media.Approved,
                Dimensions = dimensions,
                SourceId = media.SourceId,
                ImageUri = media.ImageUri,
                FileType = fileType,
                Mature = media.Mature,
                Deleted = media.Deleted
            };
        }
    }
}