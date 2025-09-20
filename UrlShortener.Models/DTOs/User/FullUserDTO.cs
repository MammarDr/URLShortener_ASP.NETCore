
﻿namespace UrlShortener.Models.DTOs.User
{

    public record FullUserDTO(
        int     ID,
        string  Email,
        int     PlanId,
        string  PlanName,
        int     MaxDailyURL,
        bool    HasCustomSlugs,
        string? PlanExpiresAt,
        string  SupportLevel
        );
}

