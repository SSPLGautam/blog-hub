using BlogApp.Core.Domain;

namespace BlogApp.Web.ViewModel;
           
public class PostsListViewModel         
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int? CategoryId { get; set; }
    public PostSortOrderEnum SortOrder { get; set; }
    public bool IsMostLiked { get; set; } = false;
    public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();
}
