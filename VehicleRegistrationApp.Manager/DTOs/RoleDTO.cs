using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRegistration.Manager.DTOs
{
    public class RoleDTO
    {
        public class RoleDto
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }

        public class PermissionDto
        {
            public string PermissionName { get; set; }
        }

        public class AssignPermissionToRoleDto
        {
            public int RoleId { get; set; }
            public int PermissionId { get; set; }
        }

        public class AssignRoleToUserDto
        {
            public int UserId { get; set; }
            public int RoleId { get; set; }
        }
    }
}
