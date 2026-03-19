using System.ComponentModel.DataAnnotations;
using BlogApp.Models;

namespace BlogApp.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }
 
        public int PostId { get; set; }

        public string UserName { get; set; }

        public DateTime CommentDate { get; set; }

        public bool CanDelete { get; set; }
    }
}