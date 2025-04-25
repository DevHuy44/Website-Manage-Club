using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<Role> GetRoleAsync(int roleID);
        Task<Role> AddRoleAsync(Role role);
        Task<Boolean> UpdateRoleAsync(Role role);
        Task<Boolean> DeleteRoleAsync(int roleID);
    }
}
