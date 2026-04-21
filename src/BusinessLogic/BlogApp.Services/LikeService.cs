using BlogApp.Core.Data;
using BlogApp.Core.Data.Repositories;
using BlogApp.Core.Services;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BlogApp.Services;

public class LikeService : ILikeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILikeRepository _likesRepository;

    public LikeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _likesRepository = unitOfWork.LikesRepository;
    }

    public async Task<bool> ToggleLikeAsync(int postId, string userId)
    {
        var existing = await _likesRepository.GetLikeByUserId(postId, userId);

        // If the user hasn't liked the post yet, add a like. Otherwise, remove the existing like.
        if (existing == null)
        {
            var like = new PostLike
            {
                PostId = postId,
                UserId = userId
            };

            await _likesRepository.AddAsync(like);
            await _unitOfWork.SaveAsync();
            return true;
        }
        else
        {
                        _likesRepository.Delete(existing);
                        await _unitOfWork.SaveAsync();  
            return false;
        }
    }
    public async Task<int> GetLikesCountByPostId(int postId)
    {
        if (postId <= 0)
            return 0;

        return _likesRepository.GetLikesCountByPostId(postId);
    }
}