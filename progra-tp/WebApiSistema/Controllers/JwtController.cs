using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using SistemaRepository.Domain;
using System.Security.Claims;
using System.Text;
using WebApiSistema.DTO;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {

        //[HttpPost]
       // public IActionResult Login([FromBody]UserLoginDto user)
       // {


       // }

        private string GenerateToken(Usuario user)
        {
            var SecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("MiClaveSuperSecreta123"));

            var Credentials = new SigningCredentials(
                SecurityKey, SecurityAlgorithms.HmacSha256);

            var IdentificadorUnico=Guid.NewGuid().ToString();

            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.NombreUsuario),
                new Claim(JwtRegisteredClaimNames.Jti,IdentificadorUnico),

                new Claim(ClaimTypes.Role,user.Rol)
            };
            var Token = new JwtSecurityToken(
                issuer: "MiApi",
                audience: "MIApi",
                claims: Claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: Credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);
                

            
        }
        
    }
}
