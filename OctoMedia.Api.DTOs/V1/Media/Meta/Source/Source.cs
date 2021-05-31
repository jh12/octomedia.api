using System;

namespace OctoMedia.Api.DTOs.V1.Media.Meta.Source
{
    public class Source
    {
        public string? Title { get; set; }
        public Author? Author { get; set; }
        public Uri SiteUri { get; set; } = null!;
        public Uri? RefererUri { get; set; }
        public bool Deleted { get; set; }

        public Source()
        {
            
        }

        public Source(string? title, Author? author, Uri siteUri, Uri? refererUri, bool deleted)
        {
            Title = title;
            Author = author;
            SiteUri = siteUri;
            RefererUri = refererUri;
            Deleted = deleted;
        }

        public Source(string? title, string? authorName, string siteUri, string? refererUri, bool deleted)
        {
            Title = title;
            Author = string.IsNullOrEmpty(authorName) ? null : new Author(authorName!);
            SiteUri = new Uri(siteUri);
            RefererUri = string.IsNullOrEmpty(refererUri) ? null : new Uri(refererUri!);
            Deleted = deleted;
        }
    }
}