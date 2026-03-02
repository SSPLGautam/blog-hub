using BlogApp.Models;
using BlogApp.Repository;

namespace BlogApp.Service
{
    public class LikeService: ILikeService
    {
        private readonly ILikeRepository _likeRepo;

        public LikeService(ILikeRepository likeRepo)
        {
            _likeRepo = likeRepo;
        }

        public async Task<(bool liked, int count)> ToggleLikeAsync(
            int postId, string userId)
        {
            var existing = _likeRepo
                .GetUserLike(postId, userId);

            if (existing != null)
            {
                _likeRepo.Delete(existing);
                await _likeRepo.SaveAsync();

                var count = _likeRepo
                    .CountByPostId(postId);

                return (false, count);
            }

            var like = new PostLike
            {
                PostId = postId,
                UserId = userId
            };

            await _likeRepo.AddAsync(like);
            await _likeRepo.SaveAsync();

            var newCount =_likeRepo
                .CountByPostId(postId);

            return (true, newCount);
        }
    }
}
