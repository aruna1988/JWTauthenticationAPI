using JWTAuthdemo.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthdemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }
        [HttpGet("authenticate")]
        public IActionResult Authenticate()
        {//Generate the JWT Toekn and Return
            //claims
            var claims = new[]
            {
                new Claim("fullname","Aruna Mahesh"),
                new Claim(JwtRegisteredClaimNames.Sub,"user_ID")
            };
            var keyBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                Constants.Audience,
                Constants.Issuer,
                claims,
                notBefore:DateTime.Now,
                expires:DateTime.Now.AddHours(1),
                signingCredentials
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { accessToken= tokenString });
        }
        [HttpGet]
        [Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
