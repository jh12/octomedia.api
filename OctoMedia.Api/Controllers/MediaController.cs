using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using MimeTypes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OctoMedia.Api.Common.Exceptions;
using OctoMedia.Api.Common.Exceptions.File;
using OctoMedia.Api.Common.Models;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Responses;
using OctoMedia.Api.Utilities;
using Serilog;
using Serilog.Context;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace OctoMedia.Api.Controllers
{
    [ApiController]
    [Route("v1/media")]
    public class MediaController : ControllerBase
    {
        // Max file size (bytes);
        private const int UploadLimit = 100 * 1024 * 1024;

        private readonly IMediaRepository _mediaRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ILogger _logger;

        public MediaController(IMediaRepository mediaRepository, IFileRepository fileRepository, ILogger logger)
        {
            _mediaRepository = mediaRepository;
            _fileRepository = fileRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<KeyedMedia> GetMedia(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                KeyedMedia media = await _mediaRepository.GetMediaAsync(id, cancellationToken);

                return media;
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMedia([FromBody] Media media, CancellationToken cancellationToken)
        {
            media.Deleted = false;

            Guid newId = await _mediaRepository.CreateMediaAsync(media, cancellationToken);

            return CreatedAtAction(nameof(GetMedia), new { id = newId }, new CreatedResponse(newId));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> UpdateMedia([FromBody] KeyedMedia media, CancellationToken cancellationToken)
        {
            media.Deleted = false;

            using (LogContext.PushProperty("MediaId", media.Key))
            {
                throw new NotImplementedException();
            }
        }

        [HttpPut("{id}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveMedia(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                await _mediaRepository.SetMediaApproval(id, true, cancellationToken);
                return Ok();
            }
        }

        [HttpPut("{id}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectMedia(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                await _mediaRepository.SetMediaApproval(id, false, cancellationToken);
                return Ok();
            }
        }

        [HttpPut("{id}/mature")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MatureMedia(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                await _mediaRepository.SetMediaMature(id, true, cancellationToken);
                return Ok();
            }
        }

        [HttpPut("{id}/unmature")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnmatureMedia(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                await _mediaRepository.SetMediaMature(id, false, cancellationToken);
                return Ok();
            }
        }

        [HttpGet("{id}/file")]
        public async Task<IActionResult> GetMediaFile(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                if (!await _mediaRepository.MediaExistsAsync(id, cancellationToken))
                    return NotFound(new TextResponse("No file found with the requested id"));

                int mediaFileId = await _mediaRepository.GetMediaFileId(id, cancellationToken);

                MediaStreamMetadata mediaStreamMetadata = await _fileRepository.GetMediaAsync(mediaFileId, cancellationToken);
                string mimeType = MimeTypeMap.GetMimeType(mediaStreamMetadata.Extension);

                return File(mediaStreamMetadata.Content, mimeType, true);
            }
        }

        [HttpGet("{id}/file/small")]
        public async Task<IActionResult> GetMediaSmallFile(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                if (!await _mediaRepository.MediaExistsAsync(id, cancellationToken))
                    return NotFound(new TextResponse("No file found with the requested id"));

                int mediaFileId = await _mediaRepository.GetMediaFileId(id, cancellationToken);

                MediaStreamMetadata? mediaStreamMetadata = await _fileRepository.GetMediaAsync(mediaFileId, cancellationToken);
                await using (mediaStreamMetadata.Content)
                {
                    string mimeType = MimeTypeMap.GetMimeType(mediaStreamMetadata.Extension);

                    if (!mimeType.StartsWith("image", StringComparison.CurrentCultureIgnoreCase))
                        return BadRequest(new TextResponse("File does not support generation of small version"));

                    using Image image = await Image.LoadAsync(mediaStreamMetadata.Content);
                    image.Mutate(x => x.Resize(
                        new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(250),
                            Sampler = KnownResamplers.Lanczos2
                        }));

                    MemoryStream smallStream = new MemoryStream();
                    image.SaveAsJpeg(smallStream);

                    smallStream.Position = 0;

                    return File(smallStream, mimeType, true);
                }
            }
        }

        [HttpPost("{id}/file")]
        [RequestSizeLimit(UploadLimit)]
        public async Task<IActionResult> UploadMediaFile(Guid id, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("MediaId", id))
            {
                string contentType = Request.ContentType;
                if (Request.ContentLength > UploadLimit)
                    return new UnprocessableEntityResult();

                Request.EnableBuffering();
                string streamMimeType;
                try
                {
                    streamMimeType = await FileUtility.GetMimeType(Request.Body);
                }
                catch (MimeTypeNotSupportedException e)
                {
                    using (LogContext.PushProperty("MagicBytes", "0x" + BitConverter.ToString(e.InputBytes).Replace("-", string.Empty)))
                    {
                        _logger.Error(e, "Could not recognize mimetype for media file");
                    }
                    throw;
                }

                if (!string.Equals(streamMimeType, contentType, StringComparison.CurrentCultureIgnoreCase))
                    return BadRequest(new TextResponse("Content-Type did not match the request body"));

                int mediaFileId = await _mediaRepository.GetMediaFileId(id, cancellationToken);

                string extension = MimeTypeMap.GetExtension(contentType).Substring(1);
                string metaExtension = await _mediaRepository.GetMediaExtensionAsync(id, cancellationToken);

                if (!string.Equals(extension, metaExtension, StringComparison.CurrentCultureIgnoreCase))
                    return BadRequest(new TextResponse("Content-Type did not match the registered metadata"));

                if (await _fileRepository.MediaExistsAsync(mediaFileId, extension))
                    throw new MediaFileAlreadyExistsException(mediaFileId);

                SHA256 hashAlgorithm = SHA256.Create();

                await using (CryptoStream hashStream = new(Request.Body, hashAlgorithm, CryptoStreamMode.Read, false))
                {
                    await _fileRepository.SaveMediaAsync(mediaFileId, extension, hashStream, cancellationToken);

                    if (!hashStream.HasFlushedFinalBlock)
                        await hashStream.FlushFinalBlockAsync(cancellationToken);
                }

                await _mediaRepository.SaveMediaHash(id, hashAlgorithm.Hash!, cancellationToken);

                return Ok();
            }
        }

        [HttpGet("from/fileid/{id}")]
        public async Task<KeyedMedia> GetMediaFromFileId(int id, CancellationToken cancellationToken)
        {
            return await _mediaRepository.GetMediaFromFileId(id, cancellationToken);
        }

        [HttpGet("recent")]
        public async Task<KeyedMedia[]> GetRecentMediasWithFile(int count = 25, DateTime? after = null, CancellationToken cancellationToken = default)
        {
            return await _mediaRepository.GetRecentMediaWithFilesAsync(count, after, cancellationToken);
        }
    }
}