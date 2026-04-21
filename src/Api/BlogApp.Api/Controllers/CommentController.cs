using BlogApp.Core.Services;
using BlogApp.Models;
using BlogApp.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(CommentCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not logged in");

        if (dto.PostId <= 0 || string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest("Invalid data");

        var comment = new Comment
        {
            Content = dto.Content,
            PostId = dto.PostId,
            UserId = userId,
            CommentDate = DateTime.UtcNow   
        };

        await _commentService.AddCommentAsync(comment);

        return Ok(comment);
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetComments(int postId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");
        var comments = await _commentService.GetCommnetsByPostIdAsync(postId);

        var result = comments.Select(c => new CommentResponseDto
        {
            Id = c.Id,
            Content = c.Content,
            CommentDate = c.CommentDate,
            UserName = c.User?.UserName ?? "",
            PostId = c.PostId,
            CanDelete = isAdmin || c.UserId == userId || c.Post.CreatedByUserId == userId
        });
        return Ok(result);
    }
 
    
  
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");

        var deleted = await _commentService.DeleteCommentAsync(id, userId, isAdmin);

        if (!deleted)
            return Forbid();

        return Ok();
    }
}