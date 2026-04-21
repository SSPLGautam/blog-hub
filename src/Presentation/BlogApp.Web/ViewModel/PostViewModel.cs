using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BlogApp.Models;
using BlogApp.ViewModels;

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

    public IFormFile FeatureImage { get; set; }
    public string? FeatureImagePath { get; set; }

    public DateTime PublishedDate { get; set; } = DateTime.Now;

    public int CategoryId { get; set; }


    public string? CreatedByUserId { get; set; }

    public bool IsPublished { get; set; }

    public CategoryViewModel? Category { get; set; }

    public IEnumerable<LikeViewModel> Likes { get; set; } = new List<LikeViewModel>();

    public IEnumerable<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

    #region Utilities
       

    #endregion

}
