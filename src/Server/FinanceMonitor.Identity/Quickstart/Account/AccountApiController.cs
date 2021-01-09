using System;
using System.Threading.Tasks;
using FinanceMonitor.Identity.Models;
using FinanceMonitor.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

namespace IdentityServerHost.Quickstart.UI
{
    [ApiController]
    [Route("api/Account/[action]")]
    public class AccountApiController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountApiController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IBus bus)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<RegisterResponseViewModel> Register(RegisterViewModel request)
        {
            var isPasswordValid = await ValidatePassword(request.Password);

            if (!isPasswordValid)
                throw new Exception("Password is not valid");

            var newUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email
            };
            var user = await _userManager.CreateAsync(newUser);
            if (!user.Succeeded)
                throw new Exception("Failed to create user");

            await _userManager.AddPasswordAsync(newUser, request.Password);
            await _bus.Send(new UserCreated
            {
                Email = newUser.Email,
                UserId = newUser.Id
            });

            return new RegisterResponseViewModel();
        }


        private async Task<bool> ValidatePassword(string password)
        {
            foreach (var validator in _userManager.PasswordValidators)
            {
                var validationResult = await validator.ValidateAsync(_userManager, null, password);
                if (validationResult.Succeeded)
                    continue;
                return false;
            }

            return true;
        }
    }
}