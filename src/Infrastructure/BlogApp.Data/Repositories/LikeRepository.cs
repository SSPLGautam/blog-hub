using BlogApp.Models;
using BlogApp.Core.Data.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async  Task<PostLike?> GetLikeByUserId(int postId, string userId)
        {
            return await _context.PostLikes.FirstOrDefaultAsync(x =>
                    x.PostId == postId && x.UserId == userId);
        }

        public int GetLikesCountByPostId(int postId)
        {
            return  _context.PostLikes.Count(x => x.PostId == postId);
        }
    }
}
