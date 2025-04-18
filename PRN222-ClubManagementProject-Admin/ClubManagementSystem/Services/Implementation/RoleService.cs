using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role> AddRoleAsync(Role role)
        {
            return await _roleRepository.AddRoleAsync(role);
        }

        public async Task<Role> GetRoleAsync(int roleId)
        {
            return await _roleRepository.GetRoleAsync(roleId);
        }

        public async Task<Role> GetRoleAsync(string name)
        {
            return (await _roleRepository.GetRolesAsync()).Where(r => r.RoleName.Equals(name)).FirstOrDefault();
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _roleRepository.GetRolesAsync();
        }
    }
}
