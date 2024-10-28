using Microsoft.AspNetCore.Mvc;
using Vitahus_VideoService_Service;
using Vitahus_VideoService_Shared;

namespace Vitahus_VideoService.Controller;

[Route("api/[controller]")]
[ApiController]
public class VideoController(
    IVideoService videoService,
    ILogger<VideoController> logger,
    IWebHostEnvironment env
) : ControllerBase
{
    // GET: api/Video
    [HttpGet("getall")]
    public async Task<ActionResult<IEnumerable<Video>>> GetVideos()
    {
        if (env.IsDevelopment())
        {
            logger.LogInformation("Getting all videos");
            return Ok(await videoService.GetVideosAsync());
        }

        return BadRequest("Unauthorized");
    }

    [HttpGet("get/{id:guid}")]
    public async Task<ActionResult<Video>> GetVideo(Guid id)
    {
        logger.LogInformation("Fetching video with ID: {Id}", id);
        var video = await videoService.GetVideoAsync(id);

        return Ok(video);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Video>> PostVideo([FromBody] Video video)
    {
        if (video == null)
        {
            logger.LogError("Received null video object.");
            return BadRequest("Video object is null.");
        }

        // Log video information for debugging
        logger.LogInformation("Creating video with Title: {Title}", video.Title);

        // Generer et ID for videoen, hvis det ikke er sat
        video.Id = Guid.NewGuid();

        // Tilføj video til databasen
        await videoService.AddVideoAsync(video);

        // Returner den nyskabte video
        return CreatedAtAction("GetVideo", new { id = video.Id }, video);
    }

    // PUT: api/Video/5
    [HttpPut("/update/{id:guid}")]
    public async Task<IActionResult> PutVideo(Guid id, Video video)
    {
        if (id != video.Id)
        {
            return BadRequest();
        }

        await videoService.UpdateVideoAsync(video);

        return NoContent();
    }

    // DELETE: api/Video/5
    [HttpDelete("/delete/{id:guid}")]
    public async Task<IActionResult> DeleteVideo(Guid id)
    {
        var video = await videoService.GetVideoAsync(id);

        await videoService.DeleteVideoAsync(video);

        return NoContent();
    }
}

