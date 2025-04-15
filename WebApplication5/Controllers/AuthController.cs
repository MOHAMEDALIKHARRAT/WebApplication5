using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication5.Dto;

namespace WebApplication5.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    _logger.LogWarning("Registration attempt with existing email: {Email}", model.Email);
                    return BadRequest("User already exists");
                }

                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Username
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to register user {Email}: {Errors}", model.Email, string.Join(", ", result.Errors));
                    return BadRequest(result.Errors);
                }

                if (!string.IsNullOrEmpty(model.Role) && new[] { "Admin", "Manager", "Commercial" }.Contains(model.Role))
                {
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                else
                {
                    _logger.LogWarning("Invalid role {Role} provided for user {Email}", model.Role, model.Email);
                    return BadRequest(new { error = "Invalid role. Use Admin, Manager, or Commercial." });
                }

                _logger.LogInformation("User {Email} registered successfully with role {Role}", model.Email, model.Role);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", model.Email);
                return StatusCode(500, new { error = "Failed to register user: " + ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid login data for email {Email}", model.Email);
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    _logger.LogWarning("Invalid login attempt for email {Email}", model.Email);
                    return Unauthorized(new { error = "Invalid email or password" });
                }

                var roles = await _userManager.GetRolesAsync(user) ?? new List<string>();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = GenerateJwtToken(claims);

                if (roles.Any())
                {
                    HttpContext.Session.SetString("UserRole", roles.First());
                }

                _logger.LogInformation("User with email {Email} logged in successfully", model.Email);
                return Ok(new { Token = token, Roles = roles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user with email {Email}", model.Email);
                return StatusCode(500, new { error = "Failed to login: " + ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out successfully");
                return Ok("User logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { error = "Failed to logout: " + ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordsDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", model.Email);
                    return BadRequest("User not found");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{_configuration["FrontendUrl"]}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(model.Email)}";

                _logger.LogInformation("Password reset link generated for email: {Email}", model.Email);
                return Ok(new { Message = "Reset link sent", ResetLink = resetLink });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password for email: {Email}", model.Email);
                return StatusCode(500, new { error = "Failed to process password reset: " + ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset attempted for non-existent email: {Email}", model.Email);
                    return BadRequest("User not found");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to reset password for email {Email}: {Errors}", model.Email, string.Join(", ", result.Errors));
                    return BadRequest(result.Errors);
                }

                _logger.LogInformation("Password successfully reset for email: {Email}", model.Email);
                return Ok("Password reset successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for email: {Email}", model.Email);
                return StatusCode(500, new { error = "Failed to reset password: " + ex.Message });
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userList = users.Select(user => new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result
                }).ToList();

                _logger.LogInformation("Retrieved all users for admin");
                return Ok(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new { error = "Failed to retrieve users: " + ex.Message });
            }
        }

        [HttpGet("dashboard")]
        [Authorize]
        public IActionResult GetDashboard()
        {
            try
            {
                var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                if (!roles.Any())
                {
                    _logger.LogWarning("User attempted to access dashboard without valid role");
                    return Unauthorized("User does not have a valid role");
                }

                if (roles.Contains("Admin"))
                {
                    _logger.LogInformation("Admin dashboard access granted");
                    return Ok("Welcome Admin! Redirecting to Admin Dashboard.");
                }
                if (roles.Contains("Commercial"))
                {
                    _logger.LogInformation("Commercial dashboard access granted");
                    return Ok("Welcome Commercial! Redirecting to Commercial Dashboard.");
                }
                if (roles.Contains("Manager"))
                {
                    _logger.LogInformation("Manager dashboard access granted");
                    return Ok("Welcome Manager! Redirecting to Manager Dashboard.");
                }

                _logger.LogWarning("Invalid role attempted dashboard access");
                return Unauthorized("Invalid role");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing dashboard");
                return StatusCode(500, new { error = "Failed to access dashboard: " + ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthorized attempt to get current user");
                    return Unauthorized();
                }

                var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userName = User.Identity.Name;

                _logger.LogInformation("Current user information retrieved for {UserName}", userName);
                return Ok(new { UserName = userName, Role = role });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new { error = "Failed to get current user: " + ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Token refresh attempted by unknown user");
                    return Unauthorized("User not found");
                }

                var userRoles = await _userManager.GetRolesAsync(user) ?? new List<string>();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var newToken = GenerateJwtToken(claims);

                _logger.LogInformation("Token refreshed successfully for user {UserName}", user.UserName);
                return Ok(new { Token = newToken, Roles = userRoles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { error = "Failed to refresh token: " + ex.Message });
            }
        }
        [HttpGet("commercials")]
        public async Task<IActionResult> GetAllCommercials()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync("Commercial");

                var commercialList = users.Select(user => new ApiCommercialDto
                {
                    Cref = user.Id,                 // Assuming Cref = IdentityUser.Id
                    Cnom = user.UserName,           // You might want to use a custom field instead
                    CPrenom = user.Email            // Or split name and email if needed
                }).ToList();

                _logger.LogInformation("Retrieved all Commercial users successfully.");
                return Ok(commercialList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Commercial users.");
                return StatusCode(500, new { error = "Failed to retrieve Commercial users: " + ex.Message });
            }
        }

        [HttpGet("commercialds")]
        public async Task<IActionResult> GetAllCommercialse()
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync("Commercial");

                var commercialList = users.Select(user => new
                {
                    id = user.Id,
                    name = user.UserName // or combine first and last name if needed
                });

                return Ok(commercialList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to retrieve Commercial users: " + ex.Message });
            }
        }



        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings"); // Changed from "Jwt" to "JwtSettings"
            var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
            var issuer = jwtSettings["Issuer"] ?? "DefaultIssuer";
            var audience = jwtSettings["Audience"] ?? "DefaultAudience";
            var expiryInMinutes = int.TryParse(jwtSettings["ExpiryInMinutes"] ?? "60", out int minutes) ? minutes : 60;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}