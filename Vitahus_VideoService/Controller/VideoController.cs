using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vitahus_VideoService_Service;

namespace Vitahus_VideoService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController(
        IVideoService _videoService,
        ILogger<VideoController> _logger,
        IWebHostEnvironment _env,
        IMapper? mapper) 
        : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        // GET: api/Video
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideos()
        {
            return Ok(await _videoService.GetVideos());
        }

        // GET: api/Video/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var video = await _videoService.GetVideo(id);

            if (video == null)
            {
                return NotFound();
            }

            return Ok(video);
        }

        // POST: api/Video
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo(Video video)
        {
            await _videoService.AddVideo(video);

            return CreatedAtAction("GetVideo", new { id = video.Id }, video);
        }

        // PUT: api/Video/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, Video video)
        {
            if (id != video.Id)
            {
                return BadRequest();
            }

            await _videoService.UpdateVideo(video);

            return NoContent();
        }

        // DELETE: api/Video/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _videoService.GetVideo(id);
            if (video == null)
            {
                return NotFound();
            }

            await _videoService.DeleteVideo(video);

            return NoContent();
        }
    }
}