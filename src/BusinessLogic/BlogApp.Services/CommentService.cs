using BlogApp.Core.Data;
using BlogApp.Core.Data.Repositories;
using BlogApp.Core.Services;
using BlogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommentRepository _commentsRepository;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _commentsRepository = unitOfWork.CommentsRepository;
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        comment.CommentDate = DateTime.Now;

        await _commentsRepository.AddAsync(comment);
      

        return comment;
    }

    public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin)
    {
     
        var comment  = await _commentsRepository.GetByIdAsync(commentId, true, true);

        if (comment == null)
            return false;

        bool isCommentOwner = comment.UserId == userId;
        bool isPostOwner = comment.Post.CreatedByUserId == userId;

        if (!(isAdmin || isCommentOwner || isPostOwner))
            return false;

        _commentsRepository.Delete(comment);
       

        return true;
    }


    public async Task<List<Comment>> GetCommnetsByPostIdAsync(int postId)
    {
        return await _commentsRepository.GeetByPostIdAsync(postId);
            
    }
}