using OctoMedia.Api.DTOs.Interfaces;

namespace OctoMedia.Api.DTOs.V1.Media.Meta.Source
{
    public class SourceAttachments : IKeyed<int>
    {
        public int Key { get; set; }

        public RedditSource? Reddit { get; set; }

        public SourceAttachments()
        {
        }

        public SourceAttachments(int key, RedditSource? reddit)
        {
            Key = key;
            Reddit = reddit;
        }
    }
}