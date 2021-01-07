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

        public KeyedSource(Guid key, string? title, Uri siteUri, Uri? refererUri, bool deleted) : base(title, siteUri, refererUri, deleted)
        {
            Key = key;
        }

        public KeyedSource(Guid key, string? title, string siteUri, string? refererUri, bool deleted) : base(title, siteUri, refererUri, deleted)
        {
            Key = key;
        }
    }
}