using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.Common.Models;

namespace OctoMedia.Api.Common.Repositories
{
    public interface IFileRepository
    {
        Task<MediaStreamMetadata> GetMediaAsync(Guid id, CancellationToken cancellationToken);
        Task SaveMediaAsync(Guid id, string extension, Stream stream, CancellationToken cancellationToken);
    }
}