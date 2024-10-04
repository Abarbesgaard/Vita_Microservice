namespace Vitahus_VideoService_Service;

public interface IVideoService
{
    Task<IEnumerable<Video>> GetVideos();
    Task<Video> GetVideo(int id);
    Task AddVideo(Video video);
    Task UpdateVideo(Video video);
    Task DeleteVideo(Video video);
}