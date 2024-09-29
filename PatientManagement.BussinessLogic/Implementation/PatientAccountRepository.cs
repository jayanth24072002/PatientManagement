using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PatientManagement.BussinessLogic.Repository;
using PatientManagement.Models.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PatientManagement.BussinessLogic.Implementation
{
    public class PatientAccountRepository : IPatientAccountRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        public PatientAccountRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> SignUpAsync(SignUpModel signUpModel)
        {
            if (signUpModel.Password != signUpModel.ConfirmPassword)
            {
                return "Password and confirmation password do not match.";
            }

            var user = new IdentityUser
            {
                UserName = signUpModel.Email,
                Email = signUpModel.Email
            };
            var result = await _userManager.CreateAsync(user, signUpModel.Password);

            if (!result.Succeeded)
            {
                return string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return "User created successfully";
        }

        public async Task<string> SignInAsync(SignInModel signInModel)
        {
            var result = await _signInManager.PasswordSignInAsync(signInModel.Email, signInModel.Password, false, false);

            if (!result.Succeeded)
                return "Invalid login attempt";

            var user = await _userManager.FindByEmailAsync(signInModel.Email);
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtSettings = _configuration.GetSection("JWT");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
