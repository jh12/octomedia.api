namespace OctoMedia.Api.Common.Options
{
    public class MongoDBOptions
    {
        public const string Key = "MongoDB";

        public string Server { get; set; } = null!;
        public string Database { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}