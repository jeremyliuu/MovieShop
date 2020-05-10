﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieShop.Core.ApiModels.Request;
using MovieShop.Core.ServiceInterfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using MovieShop.Core.Entites;
using System.Security.Claims;
using System.Text;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IUserService _userService;
        private IConfiguration _config;
        public AccountController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUserAsync([FromBody]UserRegisterRequestModel user)
        {
            var createdUser = await _userService.CreateUser(user);
            return Ok(createdUser);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginRequestModel loginRequest)
        {
            var user = await _userService.ValidateUser(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return Unauthorized("Please enter the correct username/password");
            }
            // once un/pw is authenticated then generate token (JWT)
            var generatedToken = GenerateJWT(user);
            return Ok(new { token = generatedToken });
        }

        // Method to create the Token (JWT) that takes User as input so that it can put user's Id, FirstName
        // LastName, Roles inside the payload of the Token
        // JWT has 3 parts
        // 1. Header part which will have the Algorithm we use for generating the Token
        // 2. Payload - Information that we want inside our Token -
        // user's Id, FirstName LastName, Roles
        // 3.WE need to have a Secret to verify the Signature, make sure you don't use same secret for other applications
        // Store them securely

        private string GenerateJWT(User user)
        {
            // claims are the one which will identity the user
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                    new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    // we can store roles also
            };
            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);
            foreach (UserRole userRole in user.UserRoles){
                identityClaims.AddClaim(new Claim("role", userRole.Role.Name));
            }
            // read all the information from app.settings file to create the token
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenSettings:PrivateKey"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256Signature);
            var expires = DateTime.UtcNow.AddHours(_config.GetValue<double>("TokenSettings:ExpirationHours"));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identityClaims,
                Expires = expires,
                SigningCredentials = credentials,
                Issuer = _config["TokenSettings:Issuer"],
                Audience = _config["TokenSettings:Audience"]
            };
            var encodedJwt = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(encodedJwt);
        }

    }
}