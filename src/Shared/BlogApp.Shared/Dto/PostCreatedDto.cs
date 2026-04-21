using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Shared.Dto
{
    public  class PostCreatedDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(100)]
        public string Author { get; set; }
   
        public IFormFile FeatureImage { get; set; }

        public int CategoryId { get; set; }

    }
}
