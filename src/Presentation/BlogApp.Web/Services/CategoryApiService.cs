using System.Net.Http.Headers;

using BlogApp.Shared.Dto;


namespace BlogApp.Web.Services
{
    public class CategoryApiService
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
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
        }

        public async Task<List<CategoryDto>> GetCategories()
        {
            AddJwtToken();

            try
            {
                var response = await _http.GetAsync("Category");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }

                return await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                return new List<CategoryDto>(); 
            }
        }
    }
}