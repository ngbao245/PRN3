using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SE172266.ProductManagement.Repo.Model;
using SE172266.ProductManagement.Repo.Model.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SE172266.ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => new Error { Message = e.ErrorMessage })
                                              .ToList()
                });
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Return user data
                return Ok(new
                {
                    Email = user.Email,
                    UserId = user.Id,
                    Result = "User registered successfully"
                });
            }

            return BadRequest(new ErrorResponse
            {
                Errors = result.Errors.Select(e => new Error { Message = e.Description }).ToList()
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => new Error { Message = e.ErrorMessage })
                                              .ToList()
                });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                return Ok(new
                {
                    Email = user.Email,
                    UserId = user.Id,
                    Result = "User logged in successfully"
                });
            }

            if (result.IsLockedOut)
            {
                return BadRequest(new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = "User account locked out" } }
                });
            }

            return BadRequest(new ErrorResponse
            {
                Errors = new List<Error> { new Error { Message = "Invalid login attempt" } }
            });
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new ErrorResponse
                {
                    Errors = new List<Error> { new Error { Message = "User not found" } }
                });
            }

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(model.Role));
                if (!roleResult.Succeeded)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Errors = roleResult.Errors.Select(e => new Error { Message = e.Description }).ToList()
                    });
                }
            }

            var result = await _userManager.AddToRoleAsync(user, model.Role);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    Email = user.Email,
                    UserId = user.Id,
                    Role = model.Role,
                    Result = "Role assigned successfully"
                });
            }

            return BadRequest(new ErrorResponse
            {
                Errors = result.Errors.Select(e => new Error { Message = e.Description }).ToList()
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Result = "User logged out successfully" });
        }
    }
}
