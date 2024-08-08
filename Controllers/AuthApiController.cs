using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using simple_pos_backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace simple_pos_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private SqliteConnection _connection = new SqliteConnection("Data source = simple_pos.db");

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register model)
        {
            // TODO: add hashing
            var hashedPassword = model.Password;

            var query = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
            await _connection.ExecuteAsync(query, new { model.Username, Password = hashedPassword });

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login model)
        {
            var query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            var user = await _connection.QuerySingleOrDefaultAsync<User>(query, new { model.Username, model.Password });

            if (user == null)
                return Unauthorized();

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        // Generate JWT Token when login
        private string GenerateJwtToken(User user)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not set.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
