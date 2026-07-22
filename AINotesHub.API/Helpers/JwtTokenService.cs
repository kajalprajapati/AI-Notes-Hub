using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;

namespace AINotesHub.WPF.Helpers
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        //JWT Token generation
        public string GenerateToken(string username, string Role, Guid userId)
        {
            try
            {
                var jwtSettings = _config.GetSection("Jwt");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //Role Based Authorization....
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.PreferredUsername, username),
                new Claim(ClaimTypes.Role, Role),      // 👈 include user role
                new Claim(ClaimTypes.NameIdentifier ,userId.ToString()),      // 👈 FIX: convert Guid to string
                //new Claim("id", userId.ToString()),      // 👈 FIX: convert Guid to string
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                //JWT validation configuration

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
                    signingCredentials: creds);



                return new JwtSecurityTokenHandler().WriteToken(token);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "JWTAuth creation error while generating Token");

                //return "Token generation failed.";
                return "Internal server error.";

            }

        }
    }
}
