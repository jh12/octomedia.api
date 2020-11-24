using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Media.Meta;
using OctoMedia.Api.DTOs.V1.Media.Meta.Source;

namespace OctoMedia.Api.Common.Repositories
{
    public interface IMediaRepository
    {
        #region Source

        Task<KeyedSource> GetSourceAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateSourceAsync(Source source, CancellationToken cancellationToken);
        Task UpdateSourceAsync(KeyedSource source, CancellationToken cancellationToken);

        Task<int[]> GetSourceMediaIdsAsync(int id, CancellationToken cancellationToken);


        #region Reddit Attachment

        Task AttachRedditToSource(int id, RedditSource redditSource, CancellationToken cancellationToken);

        #endregion

        #endregion

        #region Media

        Task<KeyedMedia> GetMediaAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateMediaAsync(Media media, CancellationToken cancellationToken);
        Task UpdateMediaAsync(KeyedMedia media, CancellationToken cancellationToken);

        Task<bool> MediaExistsAsync(int id, CancellationToken cancellationToken);
        Task<string> GetMediaExtensionAsync(int id, CancellationToken cancellationToken);

        #endregion
    }
}