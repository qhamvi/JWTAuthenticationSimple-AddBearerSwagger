using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using JWTAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthentication.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        [NonAction]
        public IActionResult Index()
        {
            return View();
        }
        [NonAction]
        private UserModel AuthenticateUser(UserModel model)
        {
            UserModel user = null;
            if (model.UserName == "Vi")
            {
                user = new UserModel
                {
                    UserName = "Pham Vi Vi",
                    EmailAddress = "qhamvi@gmail.com",
                    DayJoing = new System.DateTime(2022, 09, 12),
                    Password = "1234"
                };
            }
            return user;
        }

        [NonAction]
        private string GenerateJSONWebToken(UserModel userModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userModel.UserName),
                new Claim(JwtRegisteredClaimNames.Email, userModel.EmailAddress),
                new Claim("DayJoing", userModel.DayJoing.ToString("dd/MM/yyyy")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;

        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserModel login)
        {
            IActionResult result = Unauthorized();
            var user = AuthenticateUser(login);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                result = Ok(new { token = tokenString });
            }
            return result;
        }
        // GET api/values
        [Route("values")]
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            var currentUser = HttpContext.User;
            int spendingTimeWithCompany = 0;

            if (currentUser.HasClaim(c => c.Type == "DayJoing"))
            {
                DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DayJoing").Value);
                spendingTimeWithCompany = DateTime.Today.Year - date.Year;
            }

            if (spendingTimeWithCompany > 5)
            {
                return new string[] { "High Time1", "High Time2", "High Time3", "High Time4", "High Time5" };
            }
            else
            {
                return new string[] { "value1", "value2", "value3", "value4", "value5"
};
            }
        }

    }
}
