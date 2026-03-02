using BlogApp.Models;

namespace BlogApp.Service
{
    public  interface ICommentService
    {
  Task AddCommentAsync(Comment comment, string userName);
        Task DeleteCommentAsync(int id);
    }
}
