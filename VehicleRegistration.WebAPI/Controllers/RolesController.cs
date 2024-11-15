using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRegistration.Infrastructure;
using static VehicleRegistration.Manager.DTOs.RoleDTO;
using VehicleRegistration.Infrastructure.DataBaseModels.RBAC_Model;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;

namespace VehicleRegistration.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<RolesController> _logger;

        public RolesController(ApplicationDbContext dbContext, ILogger<RolesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #region Roles
        [HttpGet("Role")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_GetRoles");
                return await _dbContext.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_GetRoles : Got an error while fetching roles. {ex}", ex);
                throw;
            }
        }

        [HttpGet("Role/{roleId}")]
        public async Task<ActionResult<Role>> GetRoleById(int roleId)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_GetRoleById : {id}", roleId);
                var role = await _dbContext.Roles.FindAsync(roleId);

                if (role == null)
                {
                    _logger.LogInformation("Role Not Found of Id = {id}", roleId);
                    return NotFound();
                }

                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_GetRoleById : Got an error while getting role. {ex}", ex);
                throw;
            }
        }

        [HttpPost("Role")]
        public async Task<ActionResult<Role>> CreateRole(RoleDto roleDto)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesContoller_CreateRole.");
                var role = new Role { RoleName = roleDto.RoleName };
                _dbContext.Roles.Add(role);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleId }, role);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesContoller_CreateRole : Got an error while creating role. {ex}", ex);
                throw;
            }
        }

        [HttpPut("Role")]
        public async Task<ActionResult> UpdateRole(RoleDto roleDto)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesContoller_UpdateRole.");
                var existingRole = await _dbContext.Roles.Where(x => x.RoleId == roleDto.RoleId).FirstOrDefaultAsync();
                if (existingRole == null)
                {
                    return NotFound("Role with given Id is not found.");
                }
                existingRole.RoleName = roleDto.RoleName;
                _dbContext.Roles.Update(existingRole);
                await _dbContext.SaveChangesAsync();

                return Ok("Role Updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_UpdateRole : Got an error while updating role. {ex}", ex);
                throw;
            }
        }

        [HttpDelete("Role/{roleId}")]
        public async Task<ActionResult> DeleteRole(int roleId)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_DeleteRole");
                var role = await _dbContext.Roles.Where(x => x.RoleId == roleId).FirstOrDefaultAsync();
                if (role == null)
                {
                    return NotFound();
                }
                _dbContext.Roles.Remove(role);
                await _dbContext.SaveChangesAsync();

                return Ok("Role Deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_DeleteRole : Got an error while Deleting role. {ex}", ex);
                throw;
            }
        }
        #endregion


        #region Permissions
        [HttpGet("Permission")]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissions()
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_GetPermissions.");
                return await _dbContext.Permissions.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_GetPermissions : Got an error while fetching permissions. {ex}", ex);
                throw;
            }
        }

        [HttpPost("Permission")]
        public async Task<ActionResult<Permission>> CreatePermission(PermissionDto permissionDto)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_CreatePermission");
                var permission = new Permission { PermissionName = permissionDto.PermissionName };
                _dbContext.Permissions.Add(permission);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPermissions), new { id = permission.PermissionId }, permission);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_CreatePermission : Got an error while creating permission. {ex}", ex);
                throw;
            }
        }

        [HttpPut("Permission/{permissionId}")]
        public async Task<ActionResult> UpdatePermission(int permissionId, PermissionDto permissionDto)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_UpdatePermission.");
                var permissions = await _dbContext.Permissions.ToListAsync();
                var existingPermission = permissions.Where(x => x.PermissionId == permissionId).FirstOrDefault();
                if (existingPermission == null)
                {
                    return NotFound();
                }
                existingPermission.PermissionName = permissionDto.PermissionName;
                _dbContext.Permissions.Update(existingPermission);
                await _dbContext.SaveChangesAsync();

                return Ok("Permission details updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_UpdatePermission : Got an error while updating permissions. {ex}", ex);
                throw;
            }
        }

        [HttpDelete("Permission/{permissionId}")]
        public async Task<ActionResult> DeletePermission(int permissionId)
        {
            try
            {
                var permission = await _dbContext.Permissions.Where(x => x.PermissionId == permissionId).FirstOrDefaultAsync();
                if (permission == null)
                    return NotFound("Permission not exist.");
                _dbContext.Permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();

                return Ok("Permission deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_DeletePermission : Got an error while deleting permission. {ex}", ex);
                throw;
            }
        }
        #endregion


        #region Role_Permission
        [HttpGet("Role-Permission/{roleId}")]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissionsForRole(int roleId)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_GetPermissionsForRole : roleId = {roleId}", roleId);
                var permissions = await _dbContext.RolePermissions
                                        .Where(rp => rp.RoleId == roleId)
                                        .Select(rp => rp.Permission)
                                        .ToListAsync();
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_GetPermissionsForRole : Got an error while fetching permission for role. {ex}", ex);
                throw;
            }
        }

        [HttpPost("Role-Permission")]
        public async Task<IActionResult> AssignPermissionToRole(AssignPermissionToRoleDto dto)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_AssignPermissionToRole.");
                var rolePermission = new RolePermission
                {
                    RoleId = dto.RoleId,
                    PermissionId = dto.PermissionId
                };

                _dbContext.RolePermissions.Add(rolePermission);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPermissionsForRole), new { roleId = dto.RoleId }, rolePermission);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_AssignPermissionToRole : Got an error while adding permissions for role. {ex}", ex);
                throw;
            }
        }
        #endregion


        #region User_Role
        [HttpGet("User-Role/{userId}")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRolesForUser(int userId)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_GetRolesForUser : userId = {userId}", userId);
                var roles = await _dbContext.UserRoles
                        .Where(ur => ur.UserId == userId)
                        .Select(ur => ur.Role)
                        .ToListAsync();

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_GetRolesForUser : Got an error while getting user role. {ex}", ex);
                throw;
            }
        }

        [HttpPost("User-Role")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserDto dto)
        {
            try
            {
                _logger.LogInformation("WebAPI_RolesController_AssignRoleToUser");
                var userRole = new UserRole
                {
                    UserId = dto.UserId,
                    RoleId = dto.RoleId
                };

                _dbContext.UserRoles.Add(userRole);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetRolesForUser), new { userId = dto.UserId }, userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError("WebAPI_RolesController_AssignRoleToUser : Got an error while assigning user role. {ex}", ex);
                throw;
            }
        }
        #endregion
    }
}
