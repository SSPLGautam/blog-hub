using BlogApp.Models;
using BlogApp.Repository;

namespace BlogApp.Service
{
    public  class CommentService: ICommentService
    {
        private readonly ICommentRepository _commentRepo;

        public CommentService(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public async Task AddCommentAsync(Comment comment, string userName)
        {
            comment.UserName = userName;
            comment.CommentDate = DateTime.Now;

            await _commentRepo.AddAsync(comment);
            await _commentRepo.SaveAsync();
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment != null)
            {
                _commentRepo.Delete(comment);
                await _commentRepo.SaveAsync();
            }
        }
    }
}
