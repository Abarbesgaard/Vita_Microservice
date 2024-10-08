using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService_Service;

public interface IVideoService
{
    Task<Video?> GetVideoAsync(Guid videoId);
    Task<IEnumerable<Video>> GetVideosAsync();
    Task AddVideoAsync(Video video);
    Task UpdateVideoAsync(Video video);
    
    Task DeleteVideoAsync(Video? video);
}
