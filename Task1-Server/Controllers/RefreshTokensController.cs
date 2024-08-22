using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeEscape.Interfaces;
using SafeEscape.Models.DTO;
using SafeEscape.Models;
using System;
using System.Threading.Tasks;

namespace SafeEscape.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RefreshTokensController : ControllerBase
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RefreshTokensController(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var refreshTokens = await _refreshTokenService.GetAllAsync();
            return Ok(refreshTokens);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var refreshToken = await _refreshTokenService.GetByIdAsync(id);
            if (refreshToken == null)
            {
                return NotFound();
            }
            return Ok(refreshToken);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(RefreshTokenDto refreshTokenDto)
        {
            var refreshToken = new RefreshToken
            {
                UserId = refreshTokenDto.UserId,
                Token = refreshTokenDto.Token,
                ExpiresAt = refreshTokenDto.ExpiresAt
            };

            await _refreshTokenService.CreateAsync(refreshToken);
            return CreatedAtAction(nameof(GetById), new { id = refreshToken.Id }, refreshToken);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RefreshTokenDto refreshTokenDto)
        {
            var refreshToken = new RefreshToken
            {
                Id = id,
                UserId = refreshTokenDto.UserId,
                Token = refreshTokenDto.Token,
                ExpiresAt = refreshTokenDto.ExpiresAt,
            };

            await _refreshTokenService.UpdateAsync(id, refreshToken);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _refreshTokenService.DeleteAsync(id);
            return NoContent();
        }
    }
}
