namespace OctoMedia.Api.Common.Options
{
    public class MongoDBOptions
    {
        public const string Key = "MongoDB";

        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}