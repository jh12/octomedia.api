using System;
using System.IO;

namespace OctoMedia.Api.Common.Models
{
    public class MediaStreamMetadata
    {
        public Guid Id { get; }
        public string Extension { get; }
        public Stream Content { get; }

        public MediaStreamMetadata(Guid id, string extension, Stream content)
        {
            Id = id;
            Extension = extension;
            Content = content;
        }
    }
}