using BlogApp.Shared.Dto;

public class PostDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public string FeatureImagePath { get; set; }
    public int CategoryId { get; set; }
    public DateTime PublishedDate { get; set; }

    public string CreatedByUserId { get; set; }   
    public bool IsPublished { get; set; }         

    public CategoryDto Category { get; set; }

    public List<LikeResponseDto> Likes { get; set; }  
}