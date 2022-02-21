using ToDoList.API.Services.TokenGenerator.Models;

namespace ToDoList.API.Services.TokenGenerator.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(ClaimsData claims);
    }
}