using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlogApp.Shared.Dto;
using Microsoft.AspNetCore.Http;

namespace BlogApp.Web.Services
{
    public class LikeApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikeApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _http = factory.CreateClient("ApiClient");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<LikeResponseDto> ToggleLike(int postId)
        {
            AddJwtToken();

            var response = await _http.PostAsync($"api/like/toggle/{postId}", null);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<LikeResponseDto>();

            return result;
        }
    }
}