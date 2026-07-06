using Conexa.Domain.Entities;

namespace Conexa.Application.Common.Interfaces;

public record TokenResult(string AccessToken, DateTime ExpiresAtUtc);

public interface ITokenService
{
    TokenResult CreateToken(User user);
}
