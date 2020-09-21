using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.Common.Exceptions;
using OctoMedia.Api.Common.Models;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.FileSystem.Configuration;

namespace OctoMedia.Api.DataAccess.FileSystem.Repositories
{
    public class FileSystemFileRepository : IFileRepository
    {
        private readonly FileSystemConfiguration _configuration;
        private const int DefaultBufferSize = 81920;

        public FileSystemFileRepository(FileSystemConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<MediaStreamMetadata> GetMediaAsync(int id, CancellationToken cancellationToken)
        {
            string location = FindFileFromId(id);

            string extension = Path.GetExtension(location).Substring(1);

            FileStream fileStream = File.OpenRead(location);

            return Task.FromResult(new MediaStreamMetadata(id, extension, fileStream));
        }

        public async Task SaveMediaAsync(int id, string extension, Stream stream, CancellationToken cancellationToken)
        {
            using (stream)
            {
                string mediaLocation = GetMediaLocation(id) + $".{extension}"; 

                if (FileWithExtensionExists(mediaLocation))
                    throw new EntryAlreadyExistsException(id);

                string directory = Path.GetDirectoryName(mediaLocation);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using FileStream fileStream = File.OpenWrite(mediaLocation);
                await stream.CopyToAsync(fileStream, DefaultBufferSize, cancellationToken);
            }
        }

        private string FindFileFromId(int id)
        {
            string fileWithoutExtension = GetMediaLocation(id);

            string directory = Path.GetDirectoryName(fileWithoutExtension);

            if(!Directory.Exists(directory))
                throw new EntryNotFoundException(id);

            string filename = Path.GetFileNameWithoutExtension(fileWithoutExtension);

            string? location = Directory.GetFiles(directory, $"{filename}.*").SingleOrDefault();

            if(location == null)
                throw new EntryNotFoundException(id);

            return location;
        }

        private bool FileWithExtensionExists(string file)
        {
            string directory = Path.GetDirectoryName(file);
            if (!Directory.Exists(directory))
                return false;

            string filename = Path.GetFileNameWithoutExtension(file);

            return Directory.GetFiles(directory, $"{filename}.*").Any();
        }

        private string GetMediaLocation(int id)
        {
            string[] folderHierarchy = ExtractFolderHierarchy(id);

            string mediaRoot = _configuration.MediaLocation;

            return Path.Combine(mediaRoot, string.Join(Path.DirectorySeparatorChar.ToString(), folderHierarchy));
        }

        private string[] ExtractFolderHierarchy(int id)
        {
            const int hierarchyLevels = 4;
            const int filesPerLevel = 1000;

            int digitsPerLevel = (int) Math.Log10(filesPerLevel);
            int totalDigitWidth = hierarchyLevels * digitsPerLevel;

            string unsplitPath = id.ToString().PadLeft(totalDigitWidth, '0');

            return Enumerable.Range(0, hierarchyLevels).Select(i => unsplitPath.Substring(i * digitsPerLevel, digitsPerLevel)).ToArray();
        }
    }
}