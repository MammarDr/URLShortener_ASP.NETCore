using UrlShortener.Core.Security.Authentication;
using UrlShortener.Core.Security.Identity;
using UrlShortener.Core.Services.Interfaces;

namespace UrlShortener.Core.Services.Implementations
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUrlService> _urlService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IPlanService> _planService;
        private readonly Lazy<IPermissionService> _permissionService;
        private readonly Lazy<ITokenService> _tokenService; 
        private readonly Lazy<IAuthService> _authService;
        private readonly Lazy<IShortCodeService> _shortCodeService;
        private readonly Lazy<ILoggerManager> _loggerManager;
        private readonly IUserContextAccessor _userContext;

        public ServiceManager(
         Lazy<IUrlService> urlService,
         Lazy<IUserService> userService,
         Lazy<IPlanService> planService,
         Lazy<IPermissionService> permissionService,
         Lazy<ITokenService> tokenService,
         Lazy<IAuthService> authService,
         Lazy<IShortCodeService> shortCodeService,
         Lazy<ILoggerManager> loggerManager,
         IUserContextAccessor userContext)
        {
            _urlService = urlService;
            _userService = userService;
            _planService = planService;
            _permissionService = permissionService;
            _tokenService = tokenService;
            _authService =  authService;
            _shortCodeService = shortCodeService;
            _loggerManager = loggerManager;
            _userContext = userContext;
        }
        public IUrlService UrlService => _urlService.Value;

        public IUserService UserService => _userService.Value;

        public IPermissionService PermissionService => _permissionService.Value;

        public ITokenService TokenService =>  _tokenService.Value;

        public IAuthService AuthService => _authService.Value;

        public ILoggerManager LoggerManager => _loggerManager.Value;

        public IPlanService PlanService => _planService.Value;

        public IShortCodeService ShortCodeService => _shortCodeService.Value;


        public UserContext  User => _userContext.Identity;
        public string CorrelationId => _userContext.CorrelationId;

    }
}
