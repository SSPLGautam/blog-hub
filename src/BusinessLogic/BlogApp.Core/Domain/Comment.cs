using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

               
        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; }= DateTime.Now;

        public string Content {  get; set; }   

        [ForeignKey("Post")]
        public int PostId {  get; set; }
        public Post  Post    { get; set; }
                     
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

    }
}
