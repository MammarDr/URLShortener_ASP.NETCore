using Microsoft.AspNetCore.JsonPatch;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Models.DTOs.Plan;

namespace UrlShortener.Core.Domain.Adapters
{
    public static class PlanAdapter
    {
        public static FullPlanDTO ToFullDTO(this Plan plan)
            => new FullPlanDTO(plan.ID, plan.Name, plan.Price, plan.MaxDailyURL, plan.HasCustomSlugs, plan.UrlExpiresAfter, plan.SupportLevel);

        public static void Patch(this Plan plan, JsonPatchDocument<UpdatePlanDTO> pathDto)
        {
            var dto = new UpdatePlanDTO
            {
                Name = plan.Name,
                Price = plan.Price,
                MaxDailyURL = plan.MaxDailyURL,
                HasCustomSlugs = plan.HasCustomSlugs,
                UrlExpiresAfter = plan.UrlExpiresAfter,
                SupportLevel = plan.SupportLevel
            };

            pathDto.ApplyTo(dto);

            plan.Name = dto.Name;
            plan.Price = dto.Price;
            plan.MaxDailyURL = dto.MaxDailyURL;
            plan.HasCustomSlugs = dto.HasCustomSlugs;
            plan.UrlExpiresAfter = dto.UrlExpiresAfter;
            plan.SupportLevel = dto.SupportLevel;
        }


        public static Plan ToPlan(this CreatePlanDTO plan)
            => new Plan
            {
                Name = plan.Name,
                Price = plan.Price,
                MaxDailyURL = plan.MaxDailyURL,
                HasCustomSlugs = plan.HasCustomSlugs,
                UrlExpiresAfter = plan.UrlExpiresAfter,
                SupportLevel = plan.SupportLevel
            };

        public static Plan ToPlan(this UpdatePlanDTO plan, int id = 0)
            => new Plan
            {
                ID = id,
                Name = plan.Name,
                Price = plan.Price,
                MaxDailyURL = plan.MaxDailyURL,
                HasCustomSlugs = plan.HasCustomSlugs,
                UrlExpiresAfter = plan.UrlExpiresAfter,
                SupportLevel = plan.SupportLevel
            };
    }
}
