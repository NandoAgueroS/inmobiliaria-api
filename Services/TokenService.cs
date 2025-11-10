using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InmobiliariaAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace InmobiliariaAPI.Services
{
    public class TokenService
    {

        public static JwtSecurityToken CrearToken(IConfiguration configuration, Propietario propietario)
        {
            var secreto = configuration["TokenAuthentication:SecretKey"];
            if (string.IsNullOrEmpty(secreto))
                throw new Exception("Falta configurar TokenAuthentication:Secret");
            var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secreto));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, propietario.IdPropietario.ToString()),
                        new Claim("FullName", propietario.Nombre + " " + propietario.Apellido),
                        new Claim(ClaimTypes.Role, "Propietario"),
                    };

            var token = new JwtSecurityToken(
                issuer: configuration["TokenAuthentication:Issuer"],
                audience: configuration["TokenAuthentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credenciales
            );
            return token;
        }
    }
}
