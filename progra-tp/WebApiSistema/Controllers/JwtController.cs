using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaAcademico.Data;
using SistemaRepository.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiSistema.DTO;

namespace WebApiSistema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JwtController(AppDbContext context)
        {
                _context = context;
        }

        [HttpPost]
         public IActionResult Login([FromBody]UserLoginDto user)
        {
            var usuariolog = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == user.UserName && u.PasswordHash == user.Password);

            if (usuariolog == null)
            {
                return Unauthorized("Usuario o contraseña incorrectas");
            }
            var token = GenerateToken(usuariolog);
            return Ok(new { Token = token });

        }

        private string GenerateToken(Usuario user)
        {
            var SecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("EstaEsUnaClaveSuperSecretaYLargaDeMasDe32Caracteres123!"));

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
