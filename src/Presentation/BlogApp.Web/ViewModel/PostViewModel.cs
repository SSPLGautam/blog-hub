using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlogApp.Models;
using BlogApp.ViewModel;
using BlogApp.Web.Helper;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.Web.ViewModel;

public class PostViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "please provide Title..")]
    [MaxLength(200, ErrorMessage = "The Title Cannot exceed 200 character..")]
    public string Title { get; set; }

    [Required(ErrorMessage = "please provide Content..")]

    public string Content { get; set; }

    [Required(ErrorMessage = "The Authore is required..")]
    [MaxLength(100, ErrorMessage = "The Name Cannot exceed 100 character..")]
    public string Authore { get; set; }

    public IFormFile? FeatureImage { get; set; }

    public string? FeatureImagePath { get; set; }

    [DataType(DataType.Date)]
    public DateTime PublishedDate { get; set; } = DateTime.Now;

    [DisplayName("Category")]
    public int CategoryId { get; set; }

    public string? CreatedByUserId { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();


    #region Utilities
     
    public Post ToDataModel(PostViewModel postViewModel)
    {
        var post = new Post()
        {
            Title = postViewModel.Title,
            Content = postViewModel.Content,
            Authore = postViewModel.Authore,
            CategoryId = postViewModel.CategoryId,
            IsPublished = true,
            PublishedDate = postViewModel.PublishedDate,
            CreatedByUserId =   postViewModel.CreatedByUserId, //TODO: get the current user id
            FeatureImagePath = postViewModel.FeatureImage != null ?
            ImagesHelper.UploadFile(postViewModel.FeatureImage) : null
        };

        return post;
    }

    #endregion

}
