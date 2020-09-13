using System.Text.Json.Serialization;

namespace OctoMedia.Api.DTOs.V1.Media.Meta
{
    public class FileType
    {
        public string Extension { get; set; } = null!;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FileClass FileClass { get; set; }

        public FileType()
        {
        }

        public FileType(string extension, FileClass fileClass)
        {
            Extension = extension;
            FileClass = fileClass;
        }
    }
}