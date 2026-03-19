using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "please provide Title..")]
        [MaxLength(200, ErrorMessage = "The Title Cannot exceed 200 character..")]
        public string Title { get; set; }

        [Required(ErrorMessage = "please provide Content..")]

        public string Content { get; set; }

        [Required(ErrorMessage = "The Authore is required..")]
        [MaxLength(100, ErrorMessage = "The Name Cannot exceed 100 character..")]
        public string Authore { get; set; }

        public string FeatureImagePath { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [ForeignKey("Category")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public string? CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public IdentityUser? CreatedByUser { get; set; }

        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();

        public bool IsPublished { get; set; } = false;

    }

}
