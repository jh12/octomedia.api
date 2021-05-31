using System;
using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Media.Meta.Source
{
    public class KeyedSource : Source, IKeyed<Guid>
    {
        public Guid Key { get; set; }

        public KeyedSource()
        {
        }

        public KeyedSource(Guid key, Author? author, string? title, Uri siteUri, Uri? refererUri, bool deleted) : base(title, author, siteUri, refererUri, deleted)
        {
            Key = key;
        }

        public KeyedSource(Guid key, string? author, string? title, string siteUri, string? refererUri, bool deleted) : base(title, author, siteUri, refererUri, deleted)
        {
            Key = key;
        }
    }
}