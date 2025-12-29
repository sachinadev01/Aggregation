using ECom_Web_Api.Data;
using ECom_Web_Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Drawing;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace ECom_Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IConfiguration _configuration;
        public UserController(ECommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> UserCreated(User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "Fields are required", statusCode = 422, user = user });
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == user.Email || u.LoginId == user.LoginId);

                if (existingUser != null)
                {
                    return Conflict(new { message = "User already exists", statusCode = 409 });
                }
                DateTime now = DateTime.Now;
                user.created_at = now;
                _context.Users.Add(user);  
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(UserCreated), new { id = user.Id }, new
                {
                    message = "User created successfully",
                    statusCode = 201,
                    user = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", statusCode = 500 });
            }
        }

        [HttpPost("UserUpdated")]
        public async Task<ActionResult<User>> UserUpdated(int id, User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "Fields are required", statusCode = 422 });
                }

                var existingUser = await _context.Users.FindAsync(id);

                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found", statusCode = 404 });
                }

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.LoginId = user.LoginId;
                existingUser.phone = user.phone;
                existingUser.address = user.address;
                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "User updated successfully",
                    statusCode = 200,
                    user = existingUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", statusCode = 500 });
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found", statusCode = 404 });
                }

                return Ok(new
                {
                    message = "User retrieved successfully",
                    statusCode = 200,
                    user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", statusCode = 500 });
            }
        }


        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                if (users == null || users.Count == 0)
                {
                    return NotFound(new { message = "No users found", statusCode = 404 });
                }

                return Ok(new { message = "Users retrieved successfully", statusCode = 200, users = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", statusCode = 500 });
            }
        }

        [HttpPost("CloneUser/{id}")]
        public async Task<ActionResult<User>> CloneUser(int id)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);

                if (existingUser == null)
                {
                    return NotFound(new { message = "User not found", statusCode = 404 });
                }

                var clonedUser = new User
                {
                    Name = existingUser.Name,
                    Email = existingUser.Email,
                    LoginId = existingUser.LoginId,
                    Password = existingUser.Password,
                    phone = existingUser.phone,
                    address = existingUser.address
                };

                _context.Users.Add(clonedUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(CloneUser), new { id = clonedUser.Id }, new
                {
                    message = "User cloned successfully",
                    statusCode = 201,
                    user = clonedUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", statusCode = 500 });
            }
        }



        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found", statusCode = 404 });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully", statusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", statusCode = 500 });
            }
        }

        //[HttpPost("Login")]
        //public async Task<ActionResult> Login([FromBody] Login loginRequest)
        //{
        //    try
        //    {
        //        if(loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
        //        {
        //            return BadRequest(new { message = "Username and password are required", StatusCode = 402 });
        //        }

        //        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);

        //        if (user == null)
        //        {
        //            return Unauthorized(new { message = "Invalid credentials", statusCode = 401 });
        //        }

        //        if(user.InvalidAttempt >= 3)
        //        {
        //            return Unauthorized(new { message = "User Blocked", StatusCode = 401 });
        //        }

        //        if (user.Password != loginRequest.Password)
        //        {
        //            user.InvalidAttempt++;
        //            if(user.InvalidAttempt >= 3)
        //            {
        //                user.Remarks = "User blocked";
        //                await _context.SaveChangesAsync();
        //                return Unauthorized(new { message = "User Blocked", StatusCode = 401 });
        //            }
        //            await _context.SaveChangesAsync();
        //            return Unauthorized(new { message = "Password does not match", StatusCode = 401, invalidAttempts = user.InvalidAttempt });
        //        }

        //        if(user.InvalidAttempt < 3)
        //        {
        //            user.InvalidAttempt = 0;
        //            user.Remarks = null;
        //        }
        //        await _context.SaveChangesAsync();
        //        return Ok(new { message = "Login Successfull", statusCode = 200 });
        //    } catch (Exception ex)
        //    {
        //        return StatusCode(500);
        //    }
        //}

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] Login loginRequest)
        {
            try
            {
                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.LoginId) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    return BadRequest(new { message = "Username and password are required", StatusCode = 402 });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.LoginId == loginRequest.LoginId);

                if (user == null || user.Password != loginRequest.Password)
                {
                    return Unauthorized(new { message = "Invalid credentials", statusCode = 401 });
                }

                //if (user.InvalidAttempt >= 3)
                //{
                //    return Unauthorized(new { message = "User Blocked", StatusCode = 401 });
                //}

                //// Reset invalid attempts
                //user.InvalidAttempt = 0;
                //user.Remarks = null;
                await _context.SaveChangesAsync();

                // Token Generate Karna
                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    message = "Login Successful",
                    statusCode = 200,
                    token = token,
                    user = new
                    {
                        user.Id,
                        user.LoginId,
                        user.Email,
                        user.Name,
                        user.phone,
                        user.address,
                        //user.role,
                        user.created_at
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error" });
            }
        }

        // JWT Token Generate Karne Ka Method
        private string GenerateJwtToken(User user)
        {
            var secretKey = "MySuperStrongSecretKey12345!@#123"; // 32-byte key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.LoginId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("userId", user.Id.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: "https://localhost:7008",
                audience: "http://localhost:4200",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



       
    }
}
