namespace OctoMedia.Api.DTOs.Interfaces
{
    public interface IKeyed<T>
    {
        T Key { get; set; }
    }
}