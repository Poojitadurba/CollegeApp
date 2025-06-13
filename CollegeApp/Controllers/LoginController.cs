using CollegeApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public ActionResult Login(LoginDTO model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Please provide username and pwd");
            }
            LoginResponseDTO response = new() { Username=model.Username};
            if(model.Username=="Poojita"&&model.Password=="Poojita123")
            {
                var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JWTSecret"));
                //var issuer = _configuration.GetValue<string>("LocalIssuer");
                //var audience = _configuration.GetValue<string>("LocalAudience");
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                  //  Issuer=issuer,
                    //Audience=audience,
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name,model.Username),
                        new Claim(ClaimTypes.Role,"Admin")
                    }),
                    Expires = DateTime.UtcNow.AddHours(4),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                response.Token = tokenHandler.WriteToken(token);

            }
            else
            {
                return Ok("wrong credentials");
            }
            return Ok(response);
        }
    }


}
