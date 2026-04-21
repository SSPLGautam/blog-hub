using AutoMapper;
using BlogApp.Api.Helper;
using BlogApp.Core.Data;
using BlogApp.Core.Domain;
using BlogApp.Core.Services;
using BlogApp.Models;
using BlogApp.Shared.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogApp.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public PostController(IPostService postService, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _postService = postService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }


    [HttpGet]
    public async Task<IActionResult> GetPosts(
        [FromQuery] int? categoryId,
        [FromQuery] bool isMostLiked,
        [FromQuery] PostSortOrderEnum sortOrder,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        var (posts, totalCount) = await _postService.GetPostsAsync(
            categoryId, isMostLiked, sortOrder, page, pageSize, userId, isAdmin);

        var postDtos = _mapper.Map<List<PostResponceDto>>(posts);

        return Ok(new pagedResponceDto<PostResponceDto>
        {
            Data = postDtos,
            TotalCount = totalCount
        });
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPost(int id)
    {
        var post = await _postService.GetPostDetailAsync(id);

        if (post == null)
            return NotFound();

        var dto = _mapper.Map<PostDetailDto>(post);

        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromForm] PostCreatedDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var post = _mapper.Map<Post>(dto);

      
        if (dto.FeatureImage != null)
        {
            post.FeatureImagePath = await ImageHelper.UploadFile(dto.FeatureImage);
        }

      
        await _postService.CreatePostAsync(post);
        await _unitOfWork.SaveAsync();

        return Ok(new { message = "Post created successfully" });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromForm] PostCreatedDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        var existingPost = await _postService.GetPostDetailAsync(id);

        if (existingPost == null)
            return NotFound();

        _mapper.Map(dto, existingPost);

        if (dto.FeatureImage != null)
        {
            existingPost.FeatureImagePath = await ImageHelper.UploadFile(dto.FeatureImage);
        }

        try
        {
            await _postService.EditPostAsync(existingPost);
            await _unitOfWork.SaveAsync();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await _postService.DeletePostAsync(id);
            await _unitOfWork.SaveAsync();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return NoContent();
    }

   
    [HttpPatch("{id}/toggle-publish")]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = User.IsInRole("Admin");

        try
        {
            await _postService.TogglePublishAsync(id, userId, isAdmin);
            await _unitOfWork.SaveAsync();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return Ok(new { message = "Publish status updated" });
    }
}      