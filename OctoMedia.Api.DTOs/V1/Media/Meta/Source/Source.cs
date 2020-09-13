using System;

namespace OctoMedia.Api.DTOs.V1.Media.Meta.Source
{
    public class Source
    {
        public string? Title { get; set; }
        public Uri SiteUri { get; set; } = null!;
        public Uri? RefererUri { get; set; }
        public bool Deleted { get; set; }

        public Source()
        {
            
        }

        public Source(string? title, Uri siteUri, Uri? refererUri, bool deleted)
        {
            Title = title;
            SiteUri = siteUri;
            RefererUri = refererUri;
            Deleted = deleted;
        }

        public Source(string? title, string siteUri, string? refererUri, bool deleted)
        {
            Title = title;
            SiteUri = new Uri(siteUri);
            RefererUri = string.IsNullOrEmpty(refererUri) ? null : new Uri(refererUri);
            Deleted = deleted;
        }
    }
}