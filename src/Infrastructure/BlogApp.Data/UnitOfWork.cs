using BlogApp.Core.Data;
using BlogApp.Core.Data.Repositories;
using BlogApp.Data.Repositories;

namespace BlogApp.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IPostRepository _postsRepository;
    private ILikeRepository _likesRepository;
    private ICommentRepository _commentsRepository;
    private ICategoryRepository _categoryRepository;

    public UnitOfWork(AppDbContext context) => _context = context;

    #region Repositories
    public IPostRepository PostsRepository =>
        _postsRepository ??= new PostRepository(_context);

    public ILikeRepository LikesRepository =>
        _likesRepository ??= new LikeRepository(_context);

    public ICommentRepository CommentsRepository =>
        _commentsRepository ??= new CommentRepository(_context);

    public ICategoryRepository CategoryRepository =>
        _categoryRepository ??= new CategoryRepository(_context);
    #endregion

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}