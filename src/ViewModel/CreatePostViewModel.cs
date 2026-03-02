using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogApp.ViewModel;

public class CreatePostViewModel
{
    #region Public Properties

    //public Post post { get; set; }
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

    [DataType(DataType.Date)]
    public DateTime PublishedDate { get; set; } = DateTime.Now;

    [DisplayName("Category")]
    public int CategoryId { get; set; }


    [ValidateNever]
    public IEnumerable<SelectListItem> Categories { get; set; }

    #endregion

    #region References

    #endregion
     
    #region Utilities

    public Post ToDataModel(CreatePostViewModel postViewModel)
    {
        var post = new Post()
        {
            Title = postViewModel.Title,
            Content = postViewModel.Content,
            Authore = postViewModel.Authore,
            CategoryId = postViewModel.CategoryId
        };

        return post;
    }

    #endregion
}
