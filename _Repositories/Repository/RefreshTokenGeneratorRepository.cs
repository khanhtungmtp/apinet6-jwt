
using System.Security.Cryptography;
using apinet6._Repositories.Interfaces;
using apinet6.Models;
namespace apinet6._Repositories.Repository
{
    public class RefreshTokenGeneratorRepository : IRefreshTokenGeneratorRepository
    {
        private readonly Learn_DBContext _context;

        public RefreshTokenGeneratorRepository(Learn_DBContext context)
        {
            _context = context;
        }

        public string GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                string RefreshToken = Convert.ToBase64String(randomnumber);
                var user = _context.TblRefreshtoken.FirstOrDefault(t => t.UserId == username);
                if (user != null)
                {
                    user.RefreshToken = RefreshToken;
                    _context.SaveChanges();
                }
                else
                {
                    TblRefreshtoken tblRefreshtoken = new TblRefreshtoken()
                    {
                        UserId = username,
                        TokenId = new Random().Next().ToString(),
                        RefreshToken = RefreshToken,
                        IsActive = true
                    };
                }
                return RefreshToken;
            }
        }
    }
}