namespace Vitahus_VideoSerivce_VideoRepository;

public interface IVideoRepository
{
    Task<Video> GetVideoAsync(Guid videoId);
    Task<IEnumerable<Video>> GetVideosAsync();
    Task AddVideoAsync(Video video);
    Task UpdateVideoAsync(Video video);
    Task DeleteVideoAsync(Guid videoId);
}
