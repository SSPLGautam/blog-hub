using BlogApp.Core.Data.Repositories;

namespace BlogApp.Core.Data;

public interface IUnitOfWork
{
    #region Repositories
    IPostRepository PostsRepository { get; }
    ILikeRepository LikesRepository { get; }
    ICommentRepository CommentsRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    #endregion
    Task<int> SaveAsync();
}
