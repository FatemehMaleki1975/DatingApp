using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
       

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;

        }

        [HttpPost("register")]
        //    public async Task<ActionResult<AppUser>>Register(string username, string password)
        //    {
        // public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDto)
        // {
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            //return user;
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreatToken(user)
            };
        }
        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
        [HttpPost("login")]
        // public async Task<ActionResult<AppUser>> Login(LoginDTO loginDto)
        // {
             public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto)
        {
            var user = await _context.Users
                     .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if (user == null) return Unauthorized("Invalid Username");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            //return user;
             return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreatToken(user)
            };
        }
    }
}