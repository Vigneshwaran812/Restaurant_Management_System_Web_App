using AutoMapper;
using Azure;
using EmailService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

using Restaurant_Reservation_Management_System_Api.Data;
using Restaurant_Reservation_Management_System_Api.Dto.Auth;
using Restaurant_Reservation_Management_System_Api.Model;
using Restaurant_Reservation_Management_System_Api.Repository;

namespace Restaurant_Reservation_Management_System_Api.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenRepository _tokenRepository;
        private readonly RestaurantDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly Dictionary<string, string> _otpDictionary;
        private readonly IConfiguration _configuration;
 
        private readonly IMapper _mapper;


        public AuthService(UserManager<ApplicationUser> userManager , ITokenRepository tokenRepository , RestaurantDbContext context , IEmailSender emailSender ,  IConfiguration configuration , IMapper mapper)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
            _context = context;
            _emailSender = emailSender;
            _configuration = configuration;
       
            _mapper = mapper;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _userManager.FindByNameAsync(registerRequestDto.Name);

            //if(existingUser == null)
            //{

            //}


            existingUser = await _userManager.FindByEmailAsync(registerRequestDto.Email);


            //if (existingUser == null)
            //{


            //}
            //OtpGenerator otpGenerator = new OtpGenerator();
            //string otp = otpGenerator.GenerateOtp();

            //DateTimeOffset indianTime = DateTimeOffset.UtcNow.ToOffset(TimeZoneInfo.FindSystemTimeZoneById("India Standard Time").BaseUtcOffset);
            //DateTimeOffset otpExpiration = indianTime.AddMinutes(2);


            var user = new ApplicationUser
            {
                UserName = registerRequestDto.Name,
                Name = registerRequestDto.Name ,
                Email = registerRequestDto.Email ,
                PhoneNumber = registerRequestDto.PhoneNumber ,


            };

            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (result.Succeeded) {
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            //var message = new Message(new string[] { user.Email});
            //_emailSender.SendEmail(message);
            
     
            //Store in Registered Customer Table 

            StoreInRegisteredCustomer(user);

            return result;


        }

        

        private async void StoreInRegisteredCustomer(ApplicationUser user)
        {
            var registeredCustomer = new RegisteredCustomer()
            {
                RegisteredCustomerId = user.Id ,
                CustomerName = user.Name,
                Email = user.Email , 
                PhoneNumber = user.PhoneNumber,

            };
            _context.RegisteredCustomers.Add(registeredCustomer);

            await _context.SaveChangesAsync();
            
        }

      


        }
    }

