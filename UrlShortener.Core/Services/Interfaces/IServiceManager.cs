using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Core.Security.Authentication;
using UrlShortener.Core.Security.Identity;

namespace UrlShortener.Core.Services.Interfaces
{
    public interface IServiceManager
    {
        IUrlService UrlService { get; }
        IUserService UserService { get; }
        IPlanService PlanService { get; }
        IPermissionService PermissionService { get; }
        ITokenService TokenService { get; }
        IAuthService AuthService { get; }   
        ILoggerManager LoggerManager { get; }
        public UserContext User { get; } 
        public string CorrelationId { get; }
        IShortCodeService ShortCodeService { get; }
    }
}
