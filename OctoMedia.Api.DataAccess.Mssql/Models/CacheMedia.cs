namespace OctoMedia.Api.DataAccess.Mssql.Models
{
    public class CacheMedia
    {
        public int Id { get; set; }
        public bool? Approved { get; set; }
        public bool Deleted { get; set; }
        public string Extension { get; set; } = null!;

        public CacheMedia()
        {
        }

        public CacheMedia(int id, bool? approved, bool deleted, string extension)
        {
            Id = id;
            Approved = approved;
            Deleted = deleted;
            Extension = extension;
        }
    }
}