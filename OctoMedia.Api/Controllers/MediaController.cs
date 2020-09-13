using System;
using System.IO;
using MimeTypes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OctoMedia.Api.Common.Models;
using OctoMedia.Api.Common.Repositories;
using OctoMedia.Api.DTOs.V1.Media;
using OctoMedia.Api.DTOs.V1.Responses;
using OctoMedia.Api.Utilities;
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

        public MediaController(IMediaRepository mediaRepository, IFileRepository fileRepository)
        {
            _mediaRepository = mediaRepository;
            _fileRepository = fileRepository;
        }

        [HttpGet("{id}")]
        public async Task<KeyedMedia> GetMedia(int id, CancellationToken cancellationToken)
        {
            KeyedMedia media = await _mediaRepository.GetMediaAsync(id, cancellationToken);

            return media;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateMedia([FromBody] Media media, CancellationToken cancellationToken)
        {
            int newId = await _mediaRepository.CreateMediaAsync(media, cancellationToken);

            return CreatedAtAction(nameof(GetMedia), new { id = newId }, new CreatedResponse(newId));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<IActionResult> UpdateMedia([FromBody] KeyedMedia media, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}/file")]
        public async Task<IActionResult> GetMediaFile(int id, CancellationToken cancellationToken)
        {
            if (!await _mediaRepository.MediaExistsAsync(id, cancellationToken))
                return NotFound(new TextResponse("No file found with the requested id"));

            MediaStreamMetadata mediaStreamMetadata = await _fileRepository.GetMediaAsync(id, cancellationToken);
            string mimeType = MimeTypeMap.GetMimeType(mediaStreamMetadata.Extension);

            return File(mediaStreamMetadata.Content, mimeType, true);
        }

        [HttpGet("{id}/file/small")]
        public async Task<IActionResult> GetMediaSmallFile(int id, CancellationToken cancellationToken)
        {
            if (!await _mediaRepository.MediaExistsAsync(id, cancellationToken))
                return NotFound(new TextResponse("No file found with the requested id"));

            MediaStreamMetadata? mediaStreamMetadata = await _fileRepository.GetMediaAsync(id, cancellationToken);
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

        [HttpPost("{id}/file")]
        [RequestSizeLimit(UploadLimit)]
        public async Task<IActionResult> UploadMediaFile(int id, CancellationToken cancellationToken)
        {
            string contentType = Request.ContentType;
            if (Request.ContentLength > UploadLimit)
                return new UnprocessableEntityResult();

            Request.EnableBuffering();
            string streamMimeType = await FileUtility.GetMimeType(Request.Body);

            if (!string.Equals(streamMimeType, contentType, StringComparison.CurrentCultureIgnoreCase))
                return BadRequest(new TextResponse("Content-Type did not match the request body"));

            string extension = MimeTypeMap.GetExtension(contentType).Substring(1);
            string metaExtension = await _mediaRepository.GetMediaExtensionAsync(id, cancellationToken);

            if (!string.Equals(extension, metaExtension, StringComparison.CurrentCultureIgnoreCase))
                return BadRequest(new TextResponse("Content-Type did not match the registered metadata"));

            await _fileRepository.SaveMediaAsync(id, extension, Request.Body, cancellationToken);

            return Ok();
        }
    }
}