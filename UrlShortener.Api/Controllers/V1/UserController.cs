using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UrlShortener.Api.Autorization;
using UrlShortener.Api.Controllers;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Models.DTOs.User;
using UrlShortener.Models.Enums;


namespace UrlShortener.Controllers.V1
{

    [ApiController]
    [Route("/api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UserController(IServiceManager _sm) : Controller
    {

        


        // = Create Endpoints = //

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        [ProducesResponseType(typeof(AccessTokenDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> RefreshTokenAsync([FromBody] RefreshTokenDTO dto)
        {

            var response = await _sm.AuthService.RefreshTokenAsync(dto.RefreshToken);
            return response.Match(
                onSuccess: res => Created("", res),
                onFailure: ApiResults.Problem
            );
        }

        [AllowAnonymous]
        [HttpPost("Signup", Name = "SignUp")]
        [ProducesResponseType(typeof(AccessTokenDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> SignUpAsync([FromBody] CreateInternalUserDTO dto)
        {
            var response = await _sm.UserService.CreateAsync(dto);
            return response.Match(
                onSuccess: res => Created("", res),
                onFailure: ApiResults.Problem
            );
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(AccessTokenDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LogInAsync([FromBody] AuthenticationRequest dto)
        {
            var response = await _sm.UserService.AuthenticateUserAsync(dto);

            return response.Match(
                onSuccess: token => Created("", token),
                onFailure: ApiResults.Problem
            );
        }

        [HttpPost("/Collection", Name = "CreateUsers")]
        [CheckPermission(enResource.User, enPermission.Create)]
        [ProducesResponseType(typeof(FullUserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> CreateUsersAsync([FromBody] IEnumerable<CreateUserDTO> dtos)
        {
            var user = await _sm.UserService.CreateUsersAsync(dtos);
            return user.Match(
                onSuccess: users => CreatedAtRoute("GetUsersById", new { version = "1",  ids = String.Join(",", users.Select(u => u.ID).ToList()) }, users),
                onFailure: ApiResults.Problem
            );
        }

        [HttpPost(Name = "CreateUser")]
        [CheckPermission(enResource.User, enPermission.Create)]
        [ProducesResponseType(typeof(FullUserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> CreateUserAsync([FromBody] CreateUserDTO dto)
        {
            var user = await _sm.UserService.CreateUserAsync(dto);
            return user.Match(
                onSuccess: u => CreatedAtRoute("GetUserById", new { version = "1", id = u.ID }, u),
                onFailure: ApiResults.Problem
            );
        }

        // = Get Endpoints = //

        [HttpGet]
        [HttpHead]
        [CheckPermission(enResource.User, enPermission.Read)]
        [ProducesResponseType(typeof(FullUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAllUsersAsync([FromQuery] UserParameters userParam, CancellationToken ct)
        {
            var usersResult = await _sm.UserService.RetrieveAllUsersWithPagingAsync(userParam, withPlan:true, asNoTracking:true, ct);

            if (usersResult.IsSuccess)
                Response.Headers["X-Pagination"] = JsonSerializer.Serialize(usersResult.Value.MetaData);

            return usersResult.Match(
               onSuccess: u => Ok(u),
               onFailure: ApiResults.Problem
               );
        }

        [HttpGet("Collection/{ids}", Name = "GetUsersById")]
        [CheckPermission(enResource.User, enPermission.Read)]
        [ProducesResponseType(typeof(FullUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUsersByIDsAsync([FromRoute] IEnumerable<int> ids, CancellationToken ct)
        {
            var user = await _sm.UserService.RetrieveUsersWithDTOAsync(ids, withPlan: true, asNoTracking: true, ct);

            return user.Match(
               onSuccess: u => Ok(u),
               onFailure: ApiResults.Problem
               );
        }

        [HttpGet("{id:int:min(1)}", Name = "GetUserById")]
        [CheckPermission(enResource.User, enPermission.Read)]
        [ProducesResponseType(typeof(FullUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserAsync([FromRoute] int id)
        {
            var user = await _sm.UserService.RetrieveUserWithDTOAsync(id);

            return user.Match(
               onSuccess: u => Ok(new FullUserDTO
                (
                   ID: u.ID,
                   Email: u.Email,
                   PlanId: u.PlanId,
                   MaxDailyURL: u.MaxDailyURL,
                   HasCustomSlugs: u.HasCustomSlugs,
                   SupportLevel: u.SupportLevel.ToString(),
                   PlanName: u.PlanName,
                   PlanExpiresAt: u.PlanExpiresAt?.ToString() ?? "Never"
               )),
               onFailure: ApiResults.Problem
               );
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(FullUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMeAsync()
        {
            var user = await _sm.UserService.RetrieveOwnedUserWithDTOAsync();
            return user.Match(
               onSuccess: u => Ok(new FullUserDTO
               (
                   ID: u.ID,
                   Email: u.Email,
                   PlanId: u.PlanId,
                   MaxDailyURL: u.MaxDailyURL,
                   HasCustomSlugs: u.HasCustomSlugs,
                   SupportLevel: u.SupportLevel.ToString(),
                   PlanName: u.PlanName,
                   PlanExpiresAt: u.PlanExpiresAt?.ToString() ?? "Never"
               )),
               onFailure: ApiResults.Problem
            );
        }

        // = Update Endpoints = //


        [HttpPatch("{id:int:min(1)}")]
        [CheckPermission(enResource.User, enPermission.Update)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateUserAsync(int id, [FromBody] JsonPatchDocument<UpdateUserDTO> patchDto)
        {
            var result = await _sm.UserService.UpdateUserAsync(id, patchDto);

            return result.Match(
                 onSuccess: () => Ok(new { Status = "UpdatedSuccesfully" }),
                 onFailure: ApiResults.Problem
                 );
        }

        [HttpPatch("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateMeAsync([FromBody] JsonPatchDocument<UpdateUserDTO> patchDto)
        {
            if (patchDto == null) return BadRequest("Invalid UPDATE request.");

            var result = await _sm.UserService.UpdateAsync(patchDto);

            return result.Match(
                onSuccess: () => Ok(new { Status = "UpdatedSuccesfully" }),
                onFailure: ApiResults.Problem
                );
        }



        // = Remove Endpoints = //

        [HttpDelete("Collection/{ids}")]
        [CheckPermission(enResource.User, enPermission.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteUsersAsync([FromRoute] IEnumerable<int> ids)
        {
            var response = await _sm.UserService.RemoveUsersAsync(ids);
            return response.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem
            );
        }

        [HttpDelete("{id:int:min(1)}")]
        [CheckPermission(enResource.User, enPermission.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteUserAsync([FromRoute] int id)
        {
            var response = await _sm.UserService.RemoveUserAsync(id);
            return response.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem
            );
        }

        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> DeleteMe()
        {
            var result = await _sm.UserService.RemoveAsync();
            return result.Match(
                 onSuccess: NoContent,
                 onFailure: ApiResults.Problem
                 );
        }


        

        
    }
}
