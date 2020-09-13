namespace OctoMedia.Api.DTOs.V1.Media.Meta
{
    public class Author
    {
        public string Username { get; set; } = null!;

        public Author()
        {
            
        }

        public Author(string username)
        {
            Username = username;
        }
    }
}