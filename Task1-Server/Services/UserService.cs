using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using SafeEscape.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Authenticates a user and generates a JWT and refresh token.
        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                return null;
            }

            if (user.IsBanned)
            {
                throw new UnauthorizedAccessException("User is banned");
            }

            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user.Id);

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticateResponse { Token = jwtToken, RefreshToken = refreshToken.Token };
        }

        // Refreshes a JWT by validating and issuing a new refresh token.
        public async Task<AuthenticateResponse> RefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken == null || refreshToken.Revoked || refreshToken.ExpiresAt <= DateTime.UtcNow) return null;

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == refreshToken.UserId);

            if (user == null) return null;

            var newRefreshToken = GenerateRefreshToken(user.Id);
            refreshToken.Revoked = true;

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            var jwtToken = GenerateJwtToken(user);

            return new AuthenticateResponse { Token = jwtToken, RefreshToken = newRefreshToken.Token };
        }

        // Revokes a given refresh token.
        public async Task RevokeTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken == null || refreshToken.Revoked || refreshToken.ExpiresAt <= DateTime.UtcNow) return;

            refreshToken.Revoked = true;
            await _context.SaveChangesAsync();
        }

        // Registers a new user.
        public async Task RegisterAsync(RegisterRequest model)
        {
            if (await _context.Users.AnyAsync(x => x.Email == model.Email))
                throw new ApplicationException("Email \"" + model.Email + "\" is already taken");

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                FullName = model.FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Retrieves all users.
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Retrieves a user by ID.
        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Creates a new user.
        public async Task CreateAsync(User user)
        {
            user.Id = Guid.NewGuid();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        // Updates an existing user.
        public async Task UpdateAsync(Guid id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            if (user.Email != existingUser.Email && await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new ApplicationException("Email \"" + user.Email + "\" is already taken");
            }

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FullName = user.FullName;
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();
        }

        // Deletes a user by ID.
        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        // Bans a user.
        public async Task BanUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.IsBanned = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Unbans a user.
        public async Task UnbanUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.IsBanned = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Updates user information based on UserDto.
        public async Task UpdateUserAsync(Guid id, UserDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.FullName = userDto.FullName;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Changes the user's password.
        public async Task ChangePasswordAsync(Guid id, ChangePasswordDto passwordDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(passwordDto.CurrentPassword, user.PasswordHash))
            {
                throw new ApplicationException("Current password is incorrect");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // Generates a JWT token for a user.
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Generates a new refresh token.
        private RefreshToken GenerateRefreshToken(Guid userId)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return new RefreshToken
                {
                    UserId = userId,
                    Token = Convert.ToBase64String(randomNumber),
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                };
            }
        }

        // Checks if a user is in a specified role.
        public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName)
        {
            var userRole = await _context.UserRoles.Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);
            return userRole != null;
        }
    }
}
