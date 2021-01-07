namespace OctoMedia.Api.Common.Options
{
    public class MssqlOptions
    {
        public const string Key = "Mssql";
        
        public string ConnectionString { get; set; } = null!;
    }
}