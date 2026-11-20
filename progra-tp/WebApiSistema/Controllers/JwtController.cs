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
using Microsoft.AspNetCore.Identity;

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
        public IActionResult Login([FromBody] UserLoginDto user)
        {
            var usuariolog = _context.Usuarios
                .FirstOrDefault(u => u.NombreUsuario == user.UserName);

            if (usuariolog == null || !usuariolog.Activo)
            {
                return Unauthorized("Usuario o contraseña incorrectos");
            }

            var hasher = new PasswordHasher<Usuario>();

            var resultado = hasher.VerifyHashedPassword(
                usuariolog,
                usuariolog.PasswordHash,
                user.Password
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Usuario o contraseña incorrectos");
            }

            var token = GenerateToken(usuariolog);

            string legajo = "";
            string nombre = usuariolog.NombreUsuario;
            string apellido = "";

            if (usuariolog.Rol == "ALUMNO")
            {
                var alumno = _context.Alumnos
                    .FirstOrDefault(a => a.UsuarioId == usuariolog.Id);

                if (alumno != null)
                {
                    legajo = alumno.Legajo;
                    nombre = alumno.Nombre;
                    apellido = alumno.Apellido;
                }
            }

            return Ok(new
            {
                Token = token,
                Rol = usuariolog.Rol,
                Legajo = legajo,
                Nombre = nombre,
                Apellido = apellido
            });
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
