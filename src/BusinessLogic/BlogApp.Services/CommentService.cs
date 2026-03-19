using BlogApp.Core.Repositories;
using BlogApp.Core.Services;
using BlogApp.Models;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;

    public CommentService(ICommentRepository commentRepo)
    {
        _commentRepo = commentRepo;
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        comment.CommentDate = DateTime.Now;

        await _commentRepo.AddAsync(comment);
        await _commentRepo.SaveAsync();

        return comment;
    }


 

    public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin)
    {
        var comment = await _commentRepo.GetCommentWithPostAsync(commentId);

        if (comment == null)
            return false;

        bool isCommentOwner = comment.UserId == userId;
        bool isPostOwner = comment.Post.CreatedByUserId == userId;

        if (!(isAdmin || isCommentOwner || isPostOwner))
            return false;

        _commentRepo.Delete(comment);
        await _commentRepo.SaveAsync();

        return true;
    }

    public async Task<List<Comment>> GetCommnetsByPostIdAsync(int postId)
    {
        return await _commentRepo.GetByPostIdAsync(postId);
    }

}