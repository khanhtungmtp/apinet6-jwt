using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using apinet6._Repositories.Interfaces;
using apinet6.Dtos;
using apinet6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace apinet6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Learn_DBContext _dBContext;
        private readonly JWTSetting _jwtsetting;
        private readonly IRefreshTokenGeneratorRepository _tokenGenerator;

        public UserController(Learn_DBContext dBContext, IOptions<JWTSetting> options, IRefreshTokenGeneratorRepository tokenGenerator = null)
        {
            _dBContext = dBContext;
            _jwtsetting = options.Value;
            _tokenGenerator = tokenGenerator;
        }

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] userParam userParam)
        {
            var user = _dBContext.TblUser.FirstOrDefault(u => u.Userid == userParam.username && u.Password == userParam.password);
            if (user == null)
                return Unauthorized();

            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(_jwtsetting.securitykey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Userid),
                        new Claim(ClaimTypes.Role, user.Role)
                    }
                ),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenhandler.WriteToken(token);
            TokenResponse tokenResponse = new TokenResponse();
            tokenResponse.JWTToken = finaltoken;
            tokenResponse.RefreshToken = _tokenGenerator.GenerateToken(userParam.username);
            return Ok(tokenResponse);
        }

        [NonAction]
        private TokenResponse Authenticate(string username, Claim[] claims)
        {

            var tokenkey = Encoding.UTF8.GetBytes(_jwtsetting.securitykey);
            var tokenhandler = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            );
            TokenResponse tokenResponse = new TokenResponse();
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = _tokenGenerator.GenerateToken(username);
            return tokenResponse;
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse tokenResponse)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenhandler.ValidateToken(tokenResponse.JWTToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtsetting.securitykey)),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out securityToken);
            var _token = securityToken as JwtSecurityToken;
            if (_token != null && !_token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return Unauthorized();
            }
            var username = principal.Identity.Name;
            var _reftable = _dBContext.TblRefreshtoken.FirstOrDefault(t => t.UserId == username && t.RefreshToken == tokenResponse.RefreshToken);
            if (_reftable == null)
                return Unauthorized();
            TokenResponse _result = Authenticate(username, principal.Claims.ToArray());
            return Ok(_result);
        }

        [HttpPost("Register")]
        public APIResponse Register([FromBody] TblUser value)
        {
            string result = string.Empty;
            try
            {
                var _emp = _dBContext.TblUser.FirstOrDefault(o => o.Userid == value.Userid);
                if (_emp != null)
                {
                    result = string.Empty;
                }
                else
                {
                    TblUser tblUser = new TblUser()
                    {
                        Name = value.Name,
                        Email = value.Email,
                        Userid = value.Userid,
                        Role = string.Empty,
                        Password = value.Password,
                        IsActive = false
                    };
                    _dBContext.TblUser.Add(tblUser);
                    _dBContext.SaveChanges();
                    result = "pass";
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return new APIResponse { keycode = string.Empty, result = result };
        }
    }


}