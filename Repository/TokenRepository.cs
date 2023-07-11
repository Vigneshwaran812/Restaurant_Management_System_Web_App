﻿using Microsoft.IdentityModel.Tokens;
using Restaurant_Reservation_Management_System_Api.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurant_Reservation_Management_System_Api.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;

        public TokenRepository(IConfiguration configuration)
        
        {
            _configuration = configuration;
        }
        public string CreateJwtToken(ApplicationUser user , List<string> roles)
        {
            //Create Claims

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)

            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //Jwt Security Token Parameters

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
                );

            //Return Token

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
