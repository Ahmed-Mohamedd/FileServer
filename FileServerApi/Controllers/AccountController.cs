using FileServer.Api.DTOs;
using FileServer.Api.Helpers;
using FileServer.Core.Entities;
using FileServer.Core.Services_Interfaces;
using FileServer.Infrastructure.Data;
using FileServerApi.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FileServerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly FileContext _fileContext;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITokenService tokenService, FileContext fileContext)
        {
            _userManager=userManager;
            _signInManager=signInManager;
            _tokenService=tokenService;
            _fileContext=fileContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                if (await AccountHelper.EmailExists(model.Email, _userManager))
                    return BadRequest("there is already an account associated with that email");

                var user = new IdentityUser
                {
                    UserName = model.Username,
                    Email = model.Email

                };

                // Store user data in AspNetUsers database table
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.User_Role);
                    await AccountHelper.InitializeNewUserQuota(user, _fileContext);
                    var userDto = new UserDto()
                    {
                        Username =  model.Username,
                        Email = model.Email,
                        Token = await _tokenService.CreateToken(user),
                    };
                    return Ok(userDto);
                }
                return BadRequest(new ApiResponse(400, result.Errors.First().ToString()));
            }

            return BadRequest(new ApiResponse(400 , "There are validation errors"));
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await AccountHelper.GetUser(dto.Email, _userManager);
            if (user == null) return Unauthorized(new ApiResponse(401));


            var result = await _signInManager.PasswordSignInAsync(user.UserName, dto.Password, dto.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var userDto = new UserDto()
                {
                    Username =  user.UserName,
                    Email = user.Email,
                    Token = await _tokenService.CreateToken(user),
                };
                return Ok(userDto);
            }
            return BadRequest(new ApiResponse(400));
        }
    }
}
