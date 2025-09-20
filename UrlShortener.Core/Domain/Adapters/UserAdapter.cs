using Microsoft.AspNetCore.JsonPatch;
using UrlShortener.Data.Entities.Plans;
using UrlShortener.Data.Entities.Users;
using UrlShortener.Models.DTOs.User;

namespace UrlShortener.Core.Domain.Adapters
{


    public static class UserModelAdapter
    {
        public static void Patch(this User user, JsonPatchDocument<UpdateUserDTO> patch)
        {
            var dto = new UpdateUserDTO(user.Email, user.Password, user.PlanId);
            patch.ApplyTo(dto);

            user.Email = dto.Email;
            user.Password = dto.Password;
            user.PlanId = dto.PlanId;
        }

        public static User ToUser(this CreateUserDTO dto, Plan plan)
            => new User
            {
                Email = dto.Email,
                Password = dto.Password,
                PlanId = plan.ID,
                PlanExpiresAt = DateTime.UtcNow.AddDays(180),
                Plan = plan
            };

        public static User ToUser(this CreateInternalUserDTO dto, Plan plan)
            => new User
            {
                Email = dto.Email,
                Password = dto.Password,
                PlanId = plan.ID,
                PlanExpiresAt = DateTime.UtcNow.AddDays(180),
                Plan = plan
            };

        public static FullUserDTO ToFullDTO(this User user)
        {
            return new FullUserDTO
            (
                ID: user.PlanId,
                Email: user.Email,
                PlanId: user.PlanId,
                PlanName: user.Plan.Name,
                HasCustomSlugs: user.Plan.HasCustomSlugs,
                MaxDailyURL: user.Plan.MaxDailyURL,
                PlanExpiresAt: user.PlanExpiresAt.HasValue ? $"{user.PlanExpiresAt.Value}" : "Never",
                SupportLevel: user.Plan.SupportLevel.ToString()
            );
        }

        public static FullUserDTO ToFullDTO(this User user, Plan plan)
        {
            user.Plan = plan;
            return user.ToFullDTO();
        }
    }
}
