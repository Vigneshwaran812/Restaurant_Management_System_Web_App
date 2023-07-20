using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Restaurant_Reservation_Management_System_Api.Dto.Auth;
using Restaurant_Reservation_Management_System_Api.Model;
using Restaurant_Reservation_Management_System_Api.Repository;
using Restaurant_Reservation_Management_System_Api.Services.Auth;
using System.Security.Claims;

namespace Restaurant_Reservation_Management_System_Api.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IAuthService _authService;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(IAuthService authService , ITokenRepository tokenRepository , UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _tokenRepository = tokenRepository;
            _userManager = userManager;
        }
        
        [HttpPost("register")]
        
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {




            if(ModelState.IsValid)
            {

                var result = await _authService.RegisterUserAsync(request);

                if (result.Succeeded)
                {
                    return Ok();
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }    
            }

            return ValidationProblem(ModelState);
            

        }

        [HttpPost]
        [Route("login")]

        public async Task<ActionResult<ServiceResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            //var result = await _authService.LoginUserAsync(loginRequestDto);
            //Check Email
            var identityUser = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            var response = new ServiceResponse<LoginResponseDto>(); 


            if(identityUser == null)
            {
                response.Success = false;
                response.Message = "User not found with the provided email address.";

                return NotFound(response);
                
            }

                //Check Password

            var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password);


            if(!checkPasswordResult)
             {
                 response.Success = false;
                 response.Message = "Incorrect Password";
                 return NotFound(response);
            }

            if (checkPasswordResult && identityUser is not null)

            {

                var jwtToken = await _tokenRepository.CreateJwtToken(identityUser);

                var loginResponseDto = new LoginResponseDto()
                {
                    CustomerId = identityUser.Id,
                    Email = loginRequestDto.Email,
                    Token = jwtToken,
                    Roles = await _userManager.GetRolesAsync(identityUser),

                };
                response.Data = loginResponseDto;
                response.Success = true;
                response.Message = "User Logged In Successfully!";

                return Ok(response);


            }
            else 
            {
                response.Success = false;
                return Unauthorized();

            }

         

            ModelState.AddModelError("", "Email Or Password incorrect");

            return ValidationProblem(ModelState);

        }

        //[HttpPost("ForgotPassword")]
        //[AllowAnonymous]

        //public async Task<ActionResult<ServiceResponse<string>>> ForgotPassword(string email)
        //{
        //    var response = await _authService.ForgotPassword(email);
        //    if (!response.Success)
        //    {
        //        return BadRequest(response);
        //    }
        //    return Ok(response);
        //}

    }
}
