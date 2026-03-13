using BlogApp.Data;
using BlogApp.Models;
using BlogApp.Core.Repositories;

namespace BlogApp.Data.Repositories
{
    public class LikeRepository: GenericRepository<PostLike>,ILikeRepository
    {
        private readonly AppDbContext _context;

        public LikeRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public PostLike GetUserLike(int postId, string userId)
        {
            return  _context.PostLikes
                .FirstOrDefault(x =>
                    x.PostId == postId && x.UserId == userId);
        }

        public int CountByPostId(int postId)
        {
            return  _context.PostLikes
                .Count(x => x.PostId == postId);
        }
    }
}
