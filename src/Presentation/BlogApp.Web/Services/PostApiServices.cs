using System.Net.Http.Headers;

using BlogApp.Shared.Dto;
using Microsoft.AspNetCore.Http;


namespace BlogApp.Web.Services
{
    public class PostApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = factory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            _httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
        public async Task<(List<PostResponceDto>, int)> GetPostsAsync(
            int? categoryId,
            bool isMostLiked,
            string sortOrder,
            int page,
            int pageSize)
        {
            var url = $"Post?categoryId={categoryId}&isMostLiked={isMostLiked}&sortOrder={sortOrder}&page={page}&pageSize={pageSize}";
            AddJwtToken();
            var response = await _httpClient.GetFromJsonAsync<pagedResponceDto<PostResponceDto>>(url);

            return (response.Data, response.TotalCount);
        }
                 
        public async Task<PostDetailDto> GetPostAsync(int id)
        {
            AddJwtToken();
            return await _httpClient.GetFromJsonAsync<PostDetailDto>($"Post/{id}");
        }

        public async Task CreatePostAsync(PostCreatedDto dto)
        {
            var form = BuildFormData(dto);
            AddJwtToken();
            var response = await _httpClient.PostAsync("Post", form);
            response.EnsureSuccessStatusCode();
        }
                
        public async Task UpdatePostAsync(PostCreatedDto dto)
        {
            var form = BuildFormData(dto);
            AddJwtToken();
            var response = await _httpClient.PutAsync($"Post/{dto.Id}", form);
            response.EnsureSuccessStatusCode();
        }
                
        public async Task DeletePostAsync(int id)
        { 
            AddJwtToken();
            var response = await _httpClient.DeleteAsync($"Post/{id}");
            response.EnsureSuccessStatusCode();
        }
    
        private MultipartFormDataContent BuildFormData(PostCreatedDto dto)
        {
            var form = new MultipartFormDataContent();

            form.Add(new StringContent(dto.Title), "Title");
            form.Add(new StringContent(dto.Content ?? ""), "Content");
            form.Add(new StringContent(dto.Author), "Author");
            form.Add(new StringContent(dto.CategoryId.ToString()), "CategoryId");
            form.Add(new StringContent(dto.Id.ToString()), "Id"); 

            if (dto.FeatureImage != null)
            {
                var stream = dto.FeatureImage.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.FeatureImage.ContentType);
                form.Add(fileContent, "FeatureImage", dto.FeatureImage.FileName);
            }
            
            return form;
        }
    }
}