using OctoMedia.Api.DataAccess.Mssql.Models;
using OctoMedia.Api.DTOs.V1.Media.Meta;
using OctoMedia.Api.DTOs.V1.Media.Meta.Source;

namespace OctoMedia.Api.DataAccess.Mssql.Mappers
{
    public static class SourceMapper
    {
        public static KeyedSource Map(DBSource source)
        {
            return new KeyedSource(
                source.Id,
                source.Title,
                source.SiteUri,
                source.RefererUri,
                source.Deleted);
        }
    }
}