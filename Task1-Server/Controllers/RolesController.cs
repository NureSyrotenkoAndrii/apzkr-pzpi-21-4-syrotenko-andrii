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
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(RoleDto roleDto)
        {
            var role = new Role
            {
                Name = roleDto.Name
            };

            await _roleService.CreateAsync(role);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, RoleDto roleDto)
        {
            var role = new Role
            {
                Id = id,
                Name = roleDto.Name
            };

            await _roleService.UpdateAsync(id, role);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
