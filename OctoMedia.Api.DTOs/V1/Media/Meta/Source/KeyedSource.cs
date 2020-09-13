using System;
using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Media.Meta.Source
{
    public class KeyedSource : Source, IKeyed<int>
    {
        public int Key { get; set; }

        public KeyedSource()
        {
        }

        public KeyedSource(int key, string? title, Uri siteUri, Uri? refererUri, bool deleted) : base(title, siteUri, refererUri, deleted)
        {
            Key = key;
        }

        public KeyedSource(int key, string? title, string siteUri, string? refererUri, bool deleted) : base(title, siteUri, refererUri, deleted)
        {
            Key = key;
        }
    }
}