using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.Common.Exceptions;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.FileSystem.Configuration;
using OctoMedia.Api.DTOs.V1.State;

namespace OctoMedia.Api.DataAccess.FileSystem.Repositories
{
    public class FileSystemStateRepository : IStateRepository
    {
        private readonly FileSystemConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private JsonWriterOptions _jsonWriterOptions;

        public FileSystemStateRepository(FileSystemConfiguration configuration)
        {
            _configuration = configuration;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _jsonWriterOptions = new JsonWriterOptions()
            {
                Indented = true
            };
        }

        public async Task<IEnumerable<T>> GetStatesAsync<T>(string keyPattern, CancellationToken cancellationToken) where T : State
        {
            string directory = GetDirectory<T>();

            if (!Directory.Exists(directory))
                return Enumerable.Empty<T>();

            keyPattern += ".json";

            DirectoryInfo directoryInfo = new DirectoryInfo(directory);

            FileInfo[] files = directoryInfo.GetFiles(keyPattern);

            if(!files.Any())
                throw new StateNotFoundException();

            List<T> results = new List<T>();

            foreach (FileInfo fileInfo in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using FileStream fileStream = fileInfo.OpenRead();

                T result = await JsonSerializer.DeserializeAsync<T>(fileStream, _jsonSerializerOptions, cancellationToken);
                results.Add(result);
            }

            return results;
        }

        public async Task SaveStateAsync<T>(string key, T value, CancellationToken cancellationToken) where T : State
        {
            if (!string.Equals(value.Key, key, StringComparison.CurrentCultureIgnoreCase))
                value.Key = key;

            string directory = GetDirectory<T>();
            string path = GetPath<T>(key);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using FileStream fileStream = File.Open(path, FileMode.Create, FileAccess.Write);
            await using Utf8JsonWriter writer = new Utf8JsonWriter(fileStream, _jsonWriterOptions);

            JsonSerializer.Serialize(writer, value, _jsonSerializerOptions);
        }

        public Task DeleteStateAsync<T>(string key, CancellationToken cancellationToken) where T : State
        {
            string path = GetPath<T>(key);

            if (!File.Exists(path))
                throw new StateNotFoundException("No state found with that key");

            File.Delete(path);

            return Task.CompletedTask;
        }

        private string GetDirectory<T>()
        {
            string typeName = typeof(T).Name;
            return Path.Combine(_configuration.StateLocation, typeName);
        }

        private string GetPath<T>(string key)
        {
            return Path.Combine(GetDirectory<T>(), $"{key}.json");
        }
    }
}