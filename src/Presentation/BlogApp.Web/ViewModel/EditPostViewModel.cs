using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using BlogApp.Models;

namespace BlogApp.ViewModel
{
    public class EditPostViewModel
    {
        public Post post { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IFormFile FeatureImage { get; set; }
    }
}
