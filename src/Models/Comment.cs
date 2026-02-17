using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "UserName s Required..")]
        [MaxLength(100, ErrorMessage = "The  UserName Cannot exceed 100 character..")]
        public string UserName {  get; set; }

        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; }= DateTime.Now;

        public string Content {  get; set; }

        [ForeignKey("Post")]
        public int PostId {  get; set; }
        public Post  Post    { get; set; }
    }
}
