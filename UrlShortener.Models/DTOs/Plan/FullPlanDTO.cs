using UrlShortener.Models.Enums;

namespace UrlShortener.Models.DTOs.Plan
{

    

    [Serializable]
    public record FullPlanDTO(int ID, string Name, decimal Price, int MaxDailyURL, bool HasCustomSlugs, int? ExpireAfter, enSupportLevel SupportLevel);
}
