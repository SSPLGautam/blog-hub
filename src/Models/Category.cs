using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage =" The Category Name is Required")]
        [MaxLength(100,ErrorMessage = "The Category name cannot  exceed 100 Character ")]
        public string Name { get; set; }

  
        public string?  Description { get; set; }
        
        public ICollection<Post> Posts { get; set; }
    }
}


