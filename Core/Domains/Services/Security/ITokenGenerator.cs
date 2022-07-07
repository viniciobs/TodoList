namespace Domains.Services.Security
{
    public interface ITokenGenerator
    {
        string GenerateToken(ClaimsData claims);
    }
}