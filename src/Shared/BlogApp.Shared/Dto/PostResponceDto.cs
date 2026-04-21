using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Shared.Dto
{
    public class PostResponceDto
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string FeatureImagePath { get; set; }
    }
}
