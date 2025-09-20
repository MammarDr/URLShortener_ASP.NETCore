using Asp.Versioning;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using UrlShortener.Api.Autorization;
using UrlShortener.Api.Controllers;
using UrlShortener.Core.Domain.Results;
using UrlShortener.Core.Services.Interfaces;
using UrlShortener.Models.DTOs.Paging;
using UrlShortener.Models.DTOs.Plan;
using UrlShortener.Models.Enums;

namespace UrlShortener.Controllers.V1
{
    
    [ApiController]
    [ApiVersion("1.0")] 
    [Route("api/v{version:apiVersion}/[controller]")]
    
    public class PlanController : Controller
    {
        private readonly IServiceManager _sm;
        private readonly ILoggerManager _logger;

        public PlanController(ILoggerManager logger, IServiceManager serviceManager)
        {
            _sm = serviceManager;
            _logger = logger;
        }

        #region Create
        [HttpPost]
        [CheckPermission(enResource.Plan, enPermission.Create)]
        [ProducesResponseType(typeof(FullPlanDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePlanAsync([FromBody] CreatePlanDTO planInfo)
        {
            var result = await _sm.PlanService.CreatePlanAsync(planInfo);

            return result.Match(
                onSuccess: res => CreatedAtRoute("GetPlanById", new { version = "1", id = res.ID }, res),
                onFailure: ApiResults.Problem
               );
        }
        

        [HttpPost("Collection")]
        [CheckPermission(enResource.Plan, enPermission.Create)]
        [ProducesResponseType(typeof(IEnumerable<FullPlanDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePlansAsync([FromBody] IEnumerable<CreatePlanDTO> dtos, CancellationToken ct)
        {
            var result = await _sm.PlanService.CreatePlansAsync(dtos, ct);
            return result.Match(
                onSuccess: res => CreatedAtRoute("GetPlansById", new { version = "1", ids = String.Join(",", res.Select(x => x.ID).ToList()) }, res),
                onFailure: ApiResults.Problem);
        }

        #endregion

        #region Read
        [HttpGet]
        [HttpHead]
        //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        //[HttpCacheValidation(MustRevalidate = false)]
        [CheckPermission(enResource.Plan, enPermission.Read)]
        [ProducesResponseType(typeof(IEnumerable<FullPlanDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllPlansAsync([FromQuery] PlanParameters planParam, CancellationToken ct)
        {
            planParam.SupportLevels ??= PlanParameters.SupportLevelList;

            var pagedResult = await _sm.PlanService.RetrieveAllPlansWithPagingAsync(planParam, asNoTracking: true, ct);

            if(pagedResult.IsSuccess)
                Response.Headers["X-Pagination"] = JsonSerializer.Serialize(pagedResult.Value.MetaData);

            return pagedResult.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }

        [HttpGet("Collection/{ids}", Name = "GetPlansById")]
        [CheckPermission(enResource.Plan, enPermission.Read)]
        [ProducesResponseType(typeof(IEnumerable<FullPlanDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlansAsync([FromRoute]IEnumerable<int> ids, CancellationToken ct)
        {
            var result = await _sm.PlanService.RetrievePlansWithDTOAsync(ids, asNoTracking: true, ct);
            return result.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }

        [HttpGet("{id:int:min(1)}", Name = "GetPlanById")]
        [CheckPermission(enResource.Plan, enPermission.Read)]
        [ProducesResponseType(typeof(FullPlanDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlanAsync([FromRoute] int id)
        {

            var result = await _sm.PlanService.RetrievePlanWithDTOAsync(id);
            return result.Match(
                onSuccess: res => Ok(res),
                onFailure: ApiResults.Problem
               );
        }
        #endregion

        #region Update

        [HttpPatch("{id:int:min(1)}")]
        [CheckPermission(enResource.Plan, enPermission.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdatePlanAsync(int id, [FromBody] JsonPatchDocument<UpdatePlanDTO> patchDto)
        {
            if (patchDto == null) return BadRequest("Invalid UPDATE request.");

            var result = await _sm.PlanService.UpdatePlanAsync(id, patchDto);
            return result.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem);
        }

        #endregion

        #region Delete

        [HttpDelete("{id:int:min(1)}")]
        [CheckPermission(enResource.Plan, enPermission.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var result = await _sm.PlanService.DeletePlanAsync(id);
            return result.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem);
        }

        [HttpDelete("Collection/{ids}")]
        [CheckPermission(enResource.Plan, enPermission.Delete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteByIDs(IEnumerable<int> ids, CancellationToken ct)
        {
            var result = await _sm.PlanService.DeleteByIDsAsync(ids, ct);
            return result.Match(
                onSuccess: NoContent,
                onFailure: ApiResults.Problem);
        }
        #endregion

    }
}
