using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using OctoMedia.Api.Common.Exceptions;
using OctoMedia.Api.Common.Extensions;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.Mssql.Factories;
using OctoMedia.Api.DataAccess.Mssql.Mappers;
using OctoMedia.Api.DataAccess.Mssql.Models;
using OctoMedia.Api.DataAccess.Mssql.Repositories.Interfaces;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Media.Meta.Source;

namespace OctoMedia.Api.DataAccess.Mssql.Repositories
{
    public class MssqlMediaRepository : IMediaRepository, IMssqlMediaRepository
    {
        private readonly MssqlConnectionFactory _connectionFactory;

        public MssqlMediaRepository(MssqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException();
        }

        #region Source

        public async Task<KeyedSource> GetSourceAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    Id    = src.Id,
    Title = src.Title,
    Interface = src.Interface,
    SiteDomain = src.SiteDomain,
    SiteUri    = src.SiteUri,
    RefererUri = src.RefererUri,
    Deleted    = src.Deleted
FROM
    OctoMedia.Source AS src
WHERE
    src.Id = @id";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);

            DBSource? source = await connection.QuerySingleOrDefaultAsync<DBSource>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));

            if (source == null || source.Deleted)
                throw new EntryNotFoundException(id);

            return SourceMapper.Map(source);
        }

        public async Task<int> CreateSourceAsync(Source source, CancellationToken cancellationToken)
        {
            const string uriSql = @"
DECLARE @FakeVar BIT; -- Used to trigger UPDATE action

MERGE OctoMedia.Source WITH (HOLDLOCK) AS TARGET
USING ( VALUES ( @title, @siteDomain, @siteUri, @refererUri) )
    AS source ( Title, SiteDomain, SiteUri, RefererUri )
    ON target.SiteUriHash = CAST( HASHBYTES('SHA2_256', source.SiteUri) AS BINARY(32) ) AND
       target.SiteUri = source.SiteUri AND
       target.Deleted = 0
WHEN MATCHED THEN 
    UPDATE SET @FakeVar = null
WHEN NOT MATCHED THEN
    INSERT ( Title, SiteDomain, SiteUri, RefererUri, Deleted )
VALUES
    ( source.Title, source.SiteDomain, source.SiteUri, source.RefererUri, 0 )
OUTPUT 
    $action AS [Action], CAST( INSERTED.ID AS INT ) AS [Id];";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);

            try
            {
                DBCreateResult result = await connection.QuerySingleAsync<DBCreateResult>(new CommandDefinition(uriSql, new
                {
                    title = source.Title?.NullIfEmpty(),
                    siteDomain = source.SiteUri.Host,
                    siteUri = source.SiteUri.AbsoluteUri,
                    refererUri = source.RefererUri?.AbsoluteUri
                }, cancellationToken: cancellationToken));

                if (result.Id == null)
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);

                if (result.Action == "INSERT")
                    return result.Id.Value;

                throw new EntryAlreadyExistsException(result.Id.Value, "An entry already exists with the supplied SiteUri");
            }
            catch (SqlException e) when (e.Number == 2627)
            {
                throw new EntryAlreadyExistsException();
            }
        }

        public Task UpdateSourceAsync(KeyedSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #region Reddit Attachment

        public async Task AttachRedditToSource(int id, RedditSource redditSource, CancellationToken cancellationToken)
        {
            const string mediaSql = @"
    INSERT INTO
    	OctoMedia.SourceReddit ( Id, Subreddit, Post, [User], Nsfw, PostedAt )
    VALUES
    	( @id, @subreddit, @post, @user, @nsfw, @postedAt );";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);

            try
            {
                await connection.ExecuteAsync(new CommandDefinition(mediaSql, new
                {
                    id,
                    subreddit = redditSource.Subreddit,
                    post = redditSource.Post,
                    user = redditSource.User,
                    nsfw = redditSource.Nsfw,
                    postedAt = redditSource.PostedAt
                },
                    cancellationToken: cancellationToken));
            }
            catch (SqlException e) when (e.Number == 547)
            {
                throw new EntryNotFoundException(id, "No Source found with the supplied id");
            }
            catch (SqlException e) when (e.Number == 2627)
            {
                throw new EntryAlreadyExistsException("A reddit attachment is already present on this source");
            }
        }

        #endregion

        #endregion

        #region Media

        public async Task<KeyedMedia> GetMediaAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = @"
SELECT
    Id                = media.Id,
    Title             = media.Title,
    [Description]     = media.Description,
    AuthorUsername    = media.AuthorUsername,
    Height            = media.Height,
    Width             = media.Width,
    SourceId          = media.SourceId,
    ImageUri          = media.ImageUri,
    FileTypeExtension = media.FileTypeExtension,
    FileTypeClass     = media.FileTypeClass,
    Mature            = media.Mature,
    Approved          = media.Approved,
    FileHash          = media.FileHash,
    CreatedAt         = media.CreatedAt,
    Deleted           = media.Deleted
FROM
    OctoMedia.Media               AS media
WHERE
    media.Id      = @id";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);

            DBMedia? media = await connection.QuerySingleOrDefaultAsync<DBMedia>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));

            if (media == null || media.Deleted)
                throw new EntryNotFoundException(id);

            return MediaMapper.Map(media);
        }

        public async Task<int> CreateMediaAsync(Media media, CancellationToken cancellationToken)
        {
            const string mediaSql = @"
IF NOT EXISTS ( SELECT -1 FROM OctoMedia.Source WHERE Id = @sourceId )
    SELECT Action = 'Failed', Id = NULL;
ELSE
    DECLARE @FakeVar BIT; -- Used to trigger UPDATE action

    MERGE OctoMedia.Media WITH (HOLDLOCK) AS target
    USING ( VALUES ( @title, @description, @authorUsername, @height, @width, @sourceId, @imageUri, @fileTypeExtension, @fileTypeClass, @mature, NULL, NULL, sysutcdatetime(), 0 ) )
        AS source ( Title, [Description], AuthorUsername, Height, Width, SourceId, ImageUri, FileTypeExtension, FileTypeClass, Mature, Approved, FileHash, CreatedAt, Deleted )
        ON target.ImageUriHash = CAST( HASHBYTES('SHA2_256', source.ImageUri) AS BINARY(32) ) AND
           target.ImageUri = source.ImageUri AND
           target.Deleted = 0
    WHEN MATCHED THEN
        UPDATE SET @FakeVar = null
    WHEN NOT MATCHED THEN
        INSERT ( Title, [Description], AuthorUsername, Height, Width, SourceId, ImageUri, FileTypeExtension, FileTypeClass, Mature, Approved, FileHash, CreatedAt, Deleted)
        VALUES ( source.Title, source.[Description], source.AuthorUsername, source.Height, source.Width, source.SourceId, source.ImageUri, source.FileTypeExtension, source.FileTypeClass, source.Mature, NULL, NULL, sysutcdatetime(), 0 )
    OUTPUT 
        $action AS [Action], CAST( INSERTED.ID AS INT ) AS [Id];";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);

            try
            {
                DBCreateResult result = await connection.QuerySingleAsync<DBCreateResult>(new CommandDefinition(mediaSql, new
                {
                    title = media.Title?.NullIfEmpty(),
                    description = media.Description?.NullIfEmpty(),
                    authorUsername = media.Author?.Username?.NullIfEmpty(),
                    height = media.Dimension?.Height,
                    width = media.Dimension?.Width,
                    sourceId = media.SourceId,
                    imageUri = media.ImageUri?.AbsoluteUri,
                    fileTypeExtension = media.FileType.Extension?.NullIfEmpty(),
                    fileTypeClass = media.FileType.FileClass,
                    mature = media.Mature
                },
                    cancellationToken: cancellationToken));

                if (!result.Id.HasValue)
                    throw new EntryNotFoundException(media.SourceId, $"No Source found with the id {media.SourceId}");

                if (result.Action == "INSERT")
                    return result.Id.Value;

                throw new EntryAlreadyExistsException(result.Id.Value, "An entry already exists with the supplied ImageUri");
            }
            catch (SqlException e) when (e.Number == 2627)
            {
                throw new EntryAlreadyExistsException();
            }
        }

        public Task UpdateMediaAsync(KeyedMedia media, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> MediaExistsAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT 1 FROM OctoMedia.Media WHERE Id = @id";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);
            int rowsFound = await connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));

            return rowsFound == 1;
        }

        public async Task<string> GetMediaExtensionAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT FileTypeExtension FROM OctoMedia.Media WHERE Id = @id";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);
            string extension = await connection.QuerySingleAsync<string>(new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));

            return extension;
        }

        public async Task<IDictionary<int, CacheMedia>> GetIdToExtensionMappingAsync(CancellationToken cancellationToken)
        {
            const string sql = @"SELECT Id = Id, Approved = Approved, Deleted = Deleted, Extension = FileTypeExtension FROM OctoMedia.Media;";

            using IDbConnection connection = await _connectionFactory.OpenConnectionAsync(cancellationToken);

            IEnumerable<CacheMedia> media = await connection.QueryAsync<CacheMedia>(new CommandDefinition(sql, cancellationToken: cancellationToken));

            return media.ToDictionary(m => m.Id);
        }

        #endregion
    }
}