using Microsoft.AspNetCore.JsonPatch;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Models.DTOs;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Plan;
using UrlShortener.Models.DTOs.User;

namespace UrlShortener.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<FullUserDTO>>>  CreateUsersAsync(IEnumerable<CreateUserDTO> dto, CancellationToken ct = default);
        Task<Result<FullUserDTO>>               CreateUserAsync(CreateUserDTO dto);
        Task<Result<AccessTokenDTO>>            CreateAsync(CreateInternalUserDTO dto);

        
    
        Task<Result<AccessTokenDTO>>            AuthenticateUserAsync(AuthenticationRequest dto);
        Task<Result<IEnumerable<User>>>         RetrieveAllUsersAsync(Result<UserParameters> UserParam, bool withPlan, bool asNoTracking, CancellationToken ct = default);
        Task<Result<PagedList<FullUserDTO>>>    RetrieveAllUsersWithPagingAsync(UserParameters UserParam, bool withPlan, bool asNoTracking, CancellationToken ct = default);
        Task<Result<IEnumerable<User?>>>        RetrieveUsersByIdAsync(IEnumerable<int> ids, bool withPlan, bool asNoTracking, CancellationToken ct = default);
        Task<Result<IEnumerable<FullUserDTO>>>  RetrieveUsersWithDTOAsync(IEnumerable<int> ids, bool withPlan, bool asNoTracking, CancellationToken ct = default);
        Task<Result<User?>>                     RetrieveUserByIdAsync(int id, bool withPlan);
        Task<Result<User>>                      RetrieveUserAndValidateExistenceAsync(int id, bool withPlan);
        Task<Result<FullUserDTO>>               RetrieveUserWithDTOAsync(int id, bool withPlan = true);
        Task<Result<User>>                      RetrieveOwnedUserAsync(bool withPlan);
        Task<Result<FullUserDTO>>               RetrieveOwnedUserWithDTOAsync(bool withPlan = true);

        Task<Result>                            UpdateUserAsync(int id, JsonPatchDocument<UpdateUserDTO> patchDto);
        Task<Result>                            UpdateAsync(JsonPatchDocument<UpdateUserDTO> patchDto);

        Task<Result>                            RemoveUserAsync(int id);
        Task<Result>                            RemoveUsersAsync(IEnumerable<int> IDs, CancellationToken ct = default);
        Task<Result>                            RemoveAsync();

    }
}
