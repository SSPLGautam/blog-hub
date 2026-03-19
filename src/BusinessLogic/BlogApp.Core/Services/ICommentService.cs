using BlogApp.Models;

namespace BlogApp.Core.Services
{
    public  interface ICommentService
    {
        Task<Comment> AddCommentAsync(Comment comment);
        //Task DeleteCommentAsync(int id);
        Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin);

        Task<List<Comment>> GetCommnetsByPostIdAsync(int postId);
    }
}
