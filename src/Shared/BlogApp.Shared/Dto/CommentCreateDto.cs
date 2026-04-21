using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Shared.Dto
{
   public class CommentCreateDto
    {
        public int PostId { get; set; }
        public string Content { get; set; }
    }
}
