using System;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Media.Meta.Source;

namespace OctoMedia.Api.Common.Repositories
{
    public interface IMediaRepository
    {
        #region Source

        Task<KeyedSource> GetSourceAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> CreateSourceAsync(Source source, CancellationToken cancellationToken);
        Task UpdateSourceAsync(KeyedSource source, CancellationToken cancellationToken);

        Task<Guid[]> GetSourceMediaIdsAsync(Guid sourceId, bool mustHaveFile, CancellationToken cancellationToken);
        Task<KeyedMedia[]> GetSourceMediasAsync(Guid sourceId, bool mustHaveFile, CancellationToken cancellationToken);

        Task<KeyedSource[]> GetSourceSampleAsync(int size, CancellationToken cancellationToken);

        #region Reddit Attachment

        Task AttachRedditToSource(Guid id, RedditSource redditSource, CancellationToken cancellationToken);

        #endregion

        #endregion

        #region Media

        Task<KeyedMedia> GetMediaAsync(Guid id, CancellationToken cancellationToken);
        Task<Guid> CreateMediaAsync(Media media, CancellationToken cancellationToken);
        Task UpdateMediaAsync(KeyedMedia media, CancellationToken cancellationToken);
        Task SetMediaApproval(Guid id, bool approved, CancellationToken cancellationToken);
        Task SetMediaMature(Guid id, bool mature, CancellationToken cancellationToken);

        Task<bool> MediaExistsAsync(Guid id, CancellationToken cancellationToken);
        Task<string> GetMediaExtensionAsync(Guid id, CancellationToken cancellationToken);

        Task<int> GetNextAvailableFileId();
        Task<int> GetMediaFileId(Guid id, CancellationToken cancellationToken);
        Task<KeyedMedia> GetMediaFromFileId(int id, CancellationToken cancellationToken);

        Task SaveMediaHash(Guid id, byte[] hashBytes, CancellationToken cancellationToken);

        Task<KeyedMedia[]> GetRecentMediaWithFilesAsync(int count, DateTime? after, CancellationToken cancellationToken);

        #endregion
    }
}