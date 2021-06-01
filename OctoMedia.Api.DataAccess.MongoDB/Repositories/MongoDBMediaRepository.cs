using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OctoMedia.Api.Common.Exceptions.Entry;
using OctoMedia.Api.Common.Exceptions.File;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DataAccess.MongoDB.Factories;
using OctoMedia.Api.DataAccess.MongoDB.Mappers;
using OctoMedia.Api.DataAccess.MongoDB.Models;
using OctoMedia.Api.DataAccess.MongoDB.Models.SourceAttachments;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Media.Meta.Source;

namespace OctoMedia.Api.DataAccess.MongoDB.Repositories
{
    public class MongoDBMediaRepository : IMediaRepository
    {
        private readonly MongoDBContextFactory _contextFactory;
        private IMongoCollection<MongoMedia> _mediaStore;
        private IMongoCollection<MongoSource> _sourceStore;
        private IMongoCollection<MongoIntCounter> _intCounterStore;

        public MongoDBMediaRepository(MongoDBContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            
            _mediaStore = _contextFactory.GetContext<MongoMedia>(MongoDBCollectionNames.MediaCollection);
            _sourceStore = _contextFactory.GetContext<MongoSource>(MongoDBCollectionNames.SourceCollection);
            _intCounterStore = _contextFactory.GetContext<MongoIntCounter>(MongoDBCollectionNames.IntCounterCollection);
        }

        public async Task<KeyedSource> GetSourceAsync(Guid id, CancellationToken cancellationToken)
        {
            MongoSource? source = await _sourceStore
                .Find(d => d.Id == id)
                .SingleOrDefaultAsync(cancellationToken);

            if (source == null)
                throw new EntryNotFoundException(id);

            return SourceMapper.Map(source);
        }

        public async Task<Guid> CreateSourceAsync(Source source, CancellationToken cancellationToken)
        {
            MongoSource? existingSource = await _sourceStore
                .Find(d => d.SiteUri == source.SiteUri && !d.Deleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (existingSource != null)
                throw new EntryAlreadyExistsException(existingSource.Id, "An entry already exists with the supplied SiteUri");

            MongoSource mongoSource = SourceMapper.Map(source);

            await _sourceStore.InsertOneAsync(mongoSource, null, cancellationToken);

            return mongoSource.Id;
        }

        public Task UpdateSourceAsync(KeyedSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid[]> GetSourceMediaIdsAsync(Guid sourceId, bool mustHaveFile, CancellationToken cancellationToken)
        {
            List<FilterDefinition<MongoMedia>> filters = new();

            FilterDefinition<MongoMedia> sourceIdFilter = Builders<MongoMedia>.Filter.Where(d => d.SourceId == sourceId);
            filters.Add(sourceIdFilter);

            if (mustHaveFile)
            {
                FilterDefinition<MongoMedia> mustHaveFileFilter = Builders<MongoMedia>.Filter.Where(d => d.File!.Hash != null);
                filters.Add(mustHaveFileFilter);
            }

            FilterDefinition<MongoMedia> definition = Builders<MongoMedia>.Filter.And(filters);

            List<Guid> mediaIds = await _mediaStore.Find(definition)
                .Project(d => d.Id)
                .ToListAsync(cancellationToken);

            return mediaIds.ToArray();
        }

        public async Task<KeyedMedia[]> GetSourceMediasAsync(Guid sourceId, bool mustHaveFile, CancellationToken cancellationToken)
        {
            List<FilterDefinition<MongoMedia>> filters = new();

            FilterDefinition<MongoMedia> sourceIdFilter = Builders<MongoMedia>.Filter.Where(d => d.SourceId == sourceId);
            filters.Add(sourceIdFilter);

            if (mustHaveFile)
            {
                FilterDefinition<MongoMedia> mustHaveFileFilter = Builders<MongoMedia>.Filter.Where(d => d.File!.Hash != null);
                filters.Add(mustHaveFileFilter);
            }

            FilterDefinition<MongoMedia> definition = Builders<MongoMedia>.Filter.And(filters);

            List<MongoMedia> medias = await _mediaStore.Find(definition)
                .ToListAsync(cancellationToken);

            return medias.Select(MediaMapper.Map).ToArray();
        }

        public async Task<KeyedSource[]> GetSourceSampleAsync(int size, CancellationToken cancellationToken)
        {
            List<MongoSource> sources = await _sourceStore
                .AsQueryable()
                .Where(s => !s.Deleted)
                .Sample(size)
                .ToListAsync(cancellationToken);

            return sources.Select(SourceMapper.Map).ToArray();
        }

        public async Task AttachRedditToSource(Guid id, RedditSource redditSource, CancellationToken cancellationToken)
        {
            MongoRedditAttachment mongoAttachment = SourceMapper.Map(redditSource);

            UpdateDefinition<MongoSource> updateDefinition = Builders<MongoSource>.Update.Set("Attachment", mongoAttachment);
            UpdateOptions updateOptions = new UpdateOptions(){IsUpsert = false};

            UpdateResult updateOneAsync = await _sourceStore
                .UpdateOneAsync(d => d.Id == id && !d.Deleted, updateDefinition, updateOptions, cancellationToken);
        }

        public async Task<KeyedMedia> GetMediaAsync(Guid id, CancellationToken cancellationToken)
        {
            MongoMedia? mongoMedia = await _mediaStore
                .Find(m => m.Id == id)
                .SingleOrDefaultAsync(cancellationToken);

            if (mongoMedia == null || mongoMedia.Deleted)
                throw new EntryNotFoundException(id);

            return MediaMapper.Map(mongoMedia);
        }

        public async Task<Guid> CreateMediaAsync(Media media, CancellationToken cancellationToken)
        {
            if (!await _sourceStore.Find(d => d.Id == media.SourceId).AnyAsync(cancellationToken))
                throw new EntryNotFoundException(media.SourceId, "No source found with the supplied SourceId");

            MongoMedia? existingMedia = await _mediaStore
                .Find(d => d.ImageUri == media.ImageUri && !d.Deleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (existingMedia != null)
                throw new EntryAlreadyExistsException(existingMedia.Id, "An entry already exists with the supplied ImageUri");

            MongoMedia mongoMedia = MediaMapper.Map(media);
            mongoMedia.CreatedAt = DateTime.UtcNow;

            await _mediaStore.InsertOneAsync(mongoMedia, null, cancellationToken);

            return mongoMedia.Id;
        }

        public Task UpdateMediaAsync(KeyedMedia media, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SetMediaApproval(Guid id, bool approved, CancellationToken cancellationToken)
        {
            UpdateDefinition<MongoMedia> updateDefinition = Builders<MongoMedia>.Update.Set(m => m.Approved, approved);

            await _mediaStore.UpdateOneAsync(m => m.Id == id, updateDefinition, cancellationToken: cancellationToken);
        }

        public async Task SetMediaMature(Guid id, bool mature, CancellationToken cancellationToken)
        {
            UpdateDefinition<MongoMedia> updateDefinition = Builders<MongoMedia>.Update.Set(m => m.Mature, mature);

            await _mediaStore.UpdateOneAsync(m => m.Id == id, updateDefinition, cancellationToken: cancellationToken);
        }

        public async Task<bool> MediaExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStore
                .Find(d => d.Id == id && !d.Deleted)
                .AnyAsync(cancellationToken);
        }

        public async Task<string> GetMediaExtensionAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _mediaStore
                .Find(d => d.Id == id && !d.Deleted && d.FileType != null)
                .Project(d => d.FileType!.Extension)
                .SingleAsync(cancellationToken);
        }

        public async Task<int> GetNextAvailableFileId()
        {
            UpdateDefinition<MongoIntCounter> updateDefinition = Builders<MongoIntCounter>.Update.Inc(c => c.Value, 1);
            var updateOptions = new FindOneAndUpdateOptions<MongoIntCounter, MongoIntCounter>{ReturnDocument = ReturnDocument.Before};

            MongoIntCounter? intCounter = await _intCounterStore.FindOneAndUpdateAsync<MongoIntCounter>(c => c.Key == "FileId", updateDefinition, updateOptions);

            if (intCounter == null)
            {
                await _intCounterStore.InsertOneAsync(new MongoIntCounter {Key = "FileId", Value = 2});
                return 1;
            }

            return intCounter.Value;
        }

        public async Task<int> GetMediaFileId(Guid id, CancellationToken cancellationToken)
        {
            MongoMediaFile? mediaFile = await _mediaStore
                .Find(d => d.Id == id && !d.Deleted)
                .Project(d => d.File)
                .SingleOrDefaultAsync(cancellationToken);

            if (mediaFile != null)
                return mediaFile.Id;

            int availableFileId = await GetNextAvailableFileId();
            UpdateDefinition<MongoMedia> updateDefinition = Builders<MongoMedia>.Update.Set(m => m.File, new MongoMediaFile {Id = availableFileId});

            await _mediaStore
                .UpdateOneAsync(d => d.Id == id && !d.Deleted, updateDefinition, cancellationToken: cancellationToken);

            return availableFileId;
        }

        public async Task<KeyedMedia> GetMediaFromFileId(int id, CancellationToken cancellationToken)
        {
            MongoMedia? mongoMedia = await _mediaStore
                .Find(m => m.File != null && m.File.Id == id)
                .SingleOrDefaultAsync(cancellationToken);

            if (mongoMedia == null || mongoMedia.Deleted)
                throw new MediaFileNotFoundException(id);

            return MediaMapper.Map(mongoMedia);
        }

        public async Task SaveMediaHash(Guid id, byte[] hashBytes, CancellationToken cancellationToken)
        {
            UpdateDefinition<MongoMedia> updateDefinition = Builders<MongoMedia>.Update.Set(m => m.File!.Hash, hashBytes);

            await _mediaStore.UpdateOneAsync(m => m.Id == id, updateDefinition, cancellationToken: cancellationToken);
        }
    }
}