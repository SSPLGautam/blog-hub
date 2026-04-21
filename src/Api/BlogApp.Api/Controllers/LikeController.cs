using System.Security.Claims;
using BlogApp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost("toggle/{postId}")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int postId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var liked = await _likeService.ToggleLikeAsync(postId, userId);
            var count = await _likeService.GetLikesCountByPostId(postId);

            return Ok(new
            {
                liked = liked,
                count = count
            });
        }
    }
}