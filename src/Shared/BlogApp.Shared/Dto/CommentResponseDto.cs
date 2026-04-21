using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace BlogApp.Shared.Dto
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CommentDate { get; set; }
        public string UserName { get; set; }
        public int PostId { get; set; }
        public bool CanDelete { get; set; }
    }
}
             