namespace Conexa.Application.Auth.Common;

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string Email,
    string Role);
