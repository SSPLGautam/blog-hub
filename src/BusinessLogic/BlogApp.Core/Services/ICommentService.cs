using BlogApp.Models;

namespace BlogApp.Core.Services
{
    public  interface ICommentService
    {
  Task AddCommentAsync(Comment comment, string userName);
        Task DeleteCommentAsync(int id);
    }
}
