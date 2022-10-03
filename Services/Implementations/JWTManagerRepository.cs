using BankAPI.Models;
using BankAPI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BankAPI.Services.Implementations
{
	public class JWTManagerRepository : IJWTManagerRepository
	{
		private readonly IConfiguration iconfiguration;

		public JWTManagerRepository(IConfiguration iconfiguration)
		{
			this.iconfiguration = iconfiguration;
		}
		public Tokens Authenticate(AuthenticateModel users)
		{
			var username = iconfiguration["Jwt:Username"];
			var password = iconfiguration["Jwt:Password"];

			Dictionary<string, string> UsersRecords = new Dictionary<string, string>
			{
				{ username,password}
			};

			if (!UsersRecords.Any(x => x.Key == users.Username && x.Value == users.Password))
			{
				return null;
			}

			// Else we generate JSON Web Token
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["Jwt:Key"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
			  {
				  new Claim(ClaimTypes.Name, users.Username)
			  }),
				Expires = DateTime.UtcNow.AddHours(2),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return new Tokens { Token = tokenHandler.WriteToken(token) };

		}
	}

}
