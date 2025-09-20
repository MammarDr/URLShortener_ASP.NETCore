
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UrlShortener.Api.Autorization;
using UrlShortener.Api.Controllers;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Data.Entities.Urls;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Url;
using UrlShortener.Models.Enums;
namespace UrlShortener.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/[Controller]")]
    [Authorize]
    public class UrlController(IServiceManager _sm, ILoggerManager _logger) : Controller
    {
        #region Create

        [HttpPost]
        [CheckPermission(enResource.Url, enPermission.Create)]
        [ProducesResponseType(typeof(FullUrlDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUrl([FromBody] CreateUrlDTO dto)
        {
            var result = await _sm.UrlService.CreateUrlAsync(dto);

            return result.Match(
                onSuccess: res => CreatedAtRoute("GetOwnedUrlById", new { version = "1", id = res.ID }, res),
                onFailure: ApiResults.Problem
               );
        }
        

        // Tip : Try to break point each hierachy call so u can get the whole picture 
        [HttpPost("Collection")]
        [CheckPermission(enResource.Url, enPermission.Create)]
        [ProducesResponseType(typeof(IEnumerable<FullUrlDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUrlsAsync([FromBody] IEnumerable<CreateUrlDTO> dtos, CancellationToken ct)
        {
            var result = await _sm.UrlService.CreateUrlsAsync(dtos, ct);
            return result.Match(
                onSuccess: res => CreatedAtRoute("GetOwnedUrlsById", new { version = "1", ids = String.Join(",", res.Select(x => x.ID).ToList()) }, res),
                onFailure: ApiResults.Problem);
        }
        #endregion
        #region Read

        [HttpGet]
        [HttpHead]
        [CheckPermission(enResource.Url, enPermission.Read)]
        [ProducesResponseType(typeof(IEnumerable<FullUrlDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllUrlsAsync([FromQuery] UrlParameters UrlParam, CancellationToken ct)
        {

            var pagedResult = await _sm.UrlService.RetrieveAllUrlsWithPagingAsync(UrlParam, ct);
            if(pagedResult.IsSuccess)
                Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagedResult.Value.MetaData);

            return pagedResult.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }

        [HttpGet("me")]
        [HttpHead("me")]
        [ProducesResponseType(typeof(IEnumerable<FullUrlDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllOwnedUrlsAsync([FromQuery] UrlParameters UrlParam, CancellationToken ct)
        {

            var pagedResult = await _sm.UrlService.RetrieveAllOwnedUrlsWithPagingAsync(UrlParam, ct);
            if (pagedResult.IsSuccess)
                Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagedResult.Value.MetaData);

            return pagedResult.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }

        [HttpGet("Collection/{ids}", Name = "GetUrlsById")]
        [CheckPermission(enResource.Url, enPermission.Read)]
        [ProducesResponseType(typeof(IEnumerable<FullUrlDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUrlsById([FromRoute]IEnumerable<int> ids, CancellationToken ct)
        {
            var result = await _sm.UrlService.RetrieveUrlsWithDTOAsync(ids, ct);
            return result.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }

        [HttpGet("Collection/me/{ids}", Name = "GetOwnedUrlsById")]
        [ProducesResponseType(typeof(IEnumerable<FullUrlDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnedUrlsById([FromRoute] IEnumerable<int> ids, CancellationToken ct)
        {
            var result = await _sm.UrlService.RetrieveOwnedUrlsWithDTOAsync(ids, ct);
            return result.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }

        [HttpGet("{id:int:min(1)}", Name = "GetUrlById")]
        [CheckPermission(enResource.Url, enPermission.Read)]
        [ProducesResponseType(typeof(FullUrlDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUrlAsync([FromRoute] int id)
        {

            var result = await _sm.UrlService.RetrieveUrlWithDTOAsync(id);
            return result.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }

        [HttpGet("me/{id:int:min(1)}", Name = "GetOwnedUrlById")]
        [ProducesResponseType(typeof(FullUrlDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOwnedUrlAsync([FromRoute] int id)
        {

            var result = await _sm.UrlService.RetrieveOwnedUrlWithDTOAsync(id);
            return result.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }
        #endregion
        #region Update

        [HttpPatch("{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateUrlAsync(int id, [FromBody] JsonPatchDocument<UpdateUrlDTO> patchDto)
        {
            if (patchDto == null) return BadRequest("Invalid UPDATE request.");

            var result = await _sm.UrlService.UpdateUrlAsync(id, patchDto);
            return result.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem);
        }


        #endregion
        #region Delete

        [HttpDelete("{id:int:min(1)}")]
        [CheckPermission(enResource.Url, enPermission.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveUrl(int id)
        {
            var result = await _sm.UrlService.RemoveUrlAsync(id);
            return result.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem);
        }
        [HttpDelete("me/{id:int:min(1)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveMyUrl(int id)
        {
            var result = await _sm.UrlService.RemoveUrlAsync(id);
            return result.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem);
        }

        [HttpDelete("Collection/{ids}")]
        [CheckPermission(enResource.Url, enPermission.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RemoveByIDs([FromRoute]IEnumerable<int> ids, CancellationToken ct)
        {
            var result = await _sm.UrlService.RemoveByIDsAsync(ids, ct);
            return result.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem);
        }
        #endregion      
    }
}
