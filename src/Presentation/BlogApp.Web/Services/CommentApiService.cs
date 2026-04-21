using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlogApp.Shared.Dto;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Web.Services
{
    public class CommentApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _http = factory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

  

        private void AddJwtToken()
        {
            var token = _httpContextAccessor?.HttpContext?.Session?.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _http.DefaultRequestHeaders.Authorization = null;
            }
        }
        public async Task<List<CommentResponseDto>> GetComments(int postId)
        {
            AddJwtToken();
            return await _http.GetFromJsonAsync<List<CommentResponseDto>>($"Comment/post/{postId}");
        }

        public async Task AddComment(CommentCreateDto dto)
        {
            AddJwtToken();

            var response = await _http.PostAsJsonAsync("Comment", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API ERROR: {error}");
            }
        }

        public async Task DeleteComment(int id)
        {
            AddJwtToken();
            await _http.DeleteAsync($"Comment/{id}");
        }
    }
}