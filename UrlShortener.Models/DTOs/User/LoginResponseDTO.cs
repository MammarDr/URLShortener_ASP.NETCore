
﻿namespace UrlShortener.Models.DTOs.User
{
    public record loginResponseDTO(string AccessToken, string RefreshToken, FullUserDTO User);
}

