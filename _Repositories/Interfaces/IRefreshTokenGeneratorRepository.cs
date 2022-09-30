
namespace apinet6._Repositories.Interfaces
{
    public interface IRefreshTokenGeneratorRepository
    {
        string GenerateToken(string username);
    }
}