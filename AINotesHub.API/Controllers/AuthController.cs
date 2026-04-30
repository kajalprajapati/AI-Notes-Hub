using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Windows;
using AINotesHub.API.Data;
using AINotesHub.Shared.DTOs;
using AINotesHub.Shared.Entities;
using AINotesHub.WPF.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using LoginRequest = AINotesHub.Shared.DTOs.LoginRequest;
using RegisterRequest = AINotesHub.Shared.DTOs.RegisterRequest;


namespace AINotesHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly NotesDbContext _context;
        private readonly JwtTokenService _jwtService;
        // 1. Declare the private field to hold the manager
        //private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(NotesDbContext context, JwtTokenService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            //_userManager = userManager; // 3. Assign it to your field
        }

        [AllowAnonymous]  // ✅ VERY IMPORTANT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            // Log login attempt
            Log.Information("Login attempt: Username = {Username}", request.UsernameOrEmail);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.UsernameOrEmail);

            if (user == null)
            {
                //return Unauthorized("User does not exist.");
                return Unauthorized(new { message = "User not found" });

            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid password.");
            }

            // 1. INPUT VALIDATION: (Model validation already runs automatically if using MVC/API controllers)
            if (!ModelState.IsValid)
            {
                Log.Warning("Login failed (Invalid model state) for Username = {Username}", request.UsernameOrEmail);
                return BadRequest(ModelState);
            }


            //var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            //var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                Log.Warning("Login failed - user not found: {Username}", request.UsernameOrEmail);
                return Unauthorized("Invalid username or password.");
            }
            System.Diagnostics.Debug.WriteLine($"Password entered: {request.Password}");
            System.Diagnostics.Debug.WriteLine($"Stored hash: {user.PasswordHash}");
            System.Diagnostics.Debug.WriteLine($"Password match: {BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)}");
            //Debug.WriteLine($"Password entered: {request.Password}");
            //Debug.WriteLine($"Stored hash: {user.PasswordHash}");


            // ✅ Verify hashed password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                Log.Warning("Login failed - incorrect password: {Username}", request.UsernameOrEmail);
                return Unauthorized("Invalid username or password.");

            }

            // ✅ Create JWT token here or continue login
            var token = _jwtService.GenerateToken(user.Username, user.Role, user.Id);

            return Ok(new LoginResponse
            {
                Token = token,
                Role = user.Role,
                Username = user.Username,
                UserId = user.Id
            });
        }

        [HttpPost("register")]
        [AllowAnonymous] // IMPORTANT: Needed for unauthenticated users
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid registration data.");

            // 🔍 Check if email already exists
            if (_context.Users.Any(u => u.Email == request.Email))
                return Conflict("Email already registered.");

            // Check if email exists
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (existingUser != null)
                return BadRequest("Email already registered.");

            // 🔍 Check if username already exists
            if (_context.Users.Any(u => u.Username == request.Username))
                return Conflict("Username already taken.");

            // 🔐 Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.Username,
                PasswordHash = hashedPassword,
                Role = "User",// default role
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            Log.Information("New user registered: {Email}", request.Email);

            return Ok("Registration successful.");
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}
