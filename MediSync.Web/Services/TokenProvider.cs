using MediSync.Web.IService;

namespace MediSync.Web.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetToken()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst("jwt_token")?.Value;
        }
    }
}