namespace OctoMedia.Api.DTOs.V1.Media.Meta.Source
{
    public class SourceAttachment
    {
        public RedditSource? Reddit { get; set; }

        public SourceAttachment()
        {
        }

        public SourceAttachment(RedditSource? reddit)
        {
            Reddit = reddit;
        }
    }
}