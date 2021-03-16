using HotelListing.Data;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HotelListing.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;

        public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var token = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token); 
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, 
            List<Claim> claims)
        {
            // Gettings our settings
            var jwtSettings = _configuration.GetSection("Jwt");

            /* Bruh...
               Here we take the value from our appsettings.json,
               convert it to double
               and then turn this into minutes
               So our token will be valid for only 15 minutes */
            var expiration = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(jwtSettings.GetSection("lifetime").Value)
            );

            // Our token with its options
            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("Issuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );

            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            // Claims are pieces of information about what you can do
            // "I claim to do able to do that..."

            /* Claims in JWT Token are used to store key data (e.g. username, timezone, or roles) 
               in the Token payload, besides the IssuedAt (i.e. iat), which is added by default. */
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            // this will fetch all the roles of a particular user in a form of a list
            var roles = await _userManager.GetRolesAsync(_user);

            // Here we extend our claims further with the info about roles
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            // Bad solution. But I don't know how to properly work with my
            // env vars
            // // var key = Environment.GetEnvironmentVariable("KEY");
            var key = "my_super_duper_secure_key";

            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // the second argument is just for telling what algorithm we used
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> ValidateUser(LoginUserDTO userDTO)
        {
            // does the user exist in the system and is the password correct?
            _user = await _userManager.FindByNameAsync(userDTO.Email);

            var validPassword = await _userManager.CheckPasswordAsync(_user, userDTO.Password);

            return (_user is not null && validPassword);
        }
    }
}
