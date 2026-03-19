using System.ComponentModel.DataAnnotations;

namespace BlogApp.Web.ViewModel;

public class CategoryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = " The Category Name is Required")]
    [MaxLength(100, ErrorMessage = "The Category name cannot  exceed 100 Character ")]
    public string Name { get; set; }
}
