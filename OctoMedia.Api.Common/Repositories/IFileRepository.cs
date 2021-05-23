using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.Common.Models;

namespace OctoMedia.Api.Common.Repositories
{
    public interface IFileRepository
    {
        Task<MediaStreamMetadata> GetMediaAsync(int id, CancellationToken cancellationToken);
        Task SaveMediaAsync(int id, string extension, Stream stream, CancellationToken cancellationToken);
        Task<bool> MediaExistsAsync(int id, string extension);
    }
}