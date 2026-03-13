using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Models
{
    public class PostLike
    {

        [Key]
        public int Id { get; set; }

        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
    }
}
