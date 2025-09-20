
﻿using UrlShortener.Models.Enums;

namespace UrlShortener.Models.DTOs.Url
{

    public record FullUrlDTO 
    (
         int ID,
         string Url,
         string ShortCode,
         string Source,
         string Title,
         int VisitCount,
         DateTime CreatedAt,
         DateTime LastModified,
         DateTime ExpiresAt,
         bool isActive
    );
}
