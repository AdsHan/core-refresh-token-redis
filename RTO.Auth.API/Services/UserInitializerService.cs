using Microsoft.AspNetCore.Identity;
using RTO.Auth.API.Data;
using RTO.Auth.API.Entities;

namespace RTO.Auth.API.Service
{
    public class UserInitializerService
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserInitializerService(AuthDbContext context, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated())
            {
                CreateUser(
                    new UserModel()
                    {
                        UserName = "adshan@gmail.com",
                        Email = "adshan@gmail.com",
                        EmailConfirmed = true
                    }, "123456");
            }
        }

        private void CreateUser(UserModel user, string password)
        {
            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var resultado = _userManager.CreateAsync(user, password).Result;
            }
        }
    }
}