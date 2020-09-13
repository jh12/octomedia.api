using System;

namespace OctoMedia.Api.DataAccess.Mssql.Models
{
    public class DBSource
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Interface { get; set; }
        public string SiteDomain { get; set; } = null!;
        public string SiteUri { get; set; } = null!;
        public string? RefererUri { get; set; }
        public bool Deleted { get; set; }
    }
}