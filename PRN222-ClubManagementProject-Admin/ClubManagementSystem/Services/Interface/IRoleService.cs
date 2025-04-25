using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetRolesAsync();

        Task<Role> GetRoleAsync(string name);

        Task<Role> GetRoleAsync(int roleId);

        Task<Role> AddRoleAsync(Role role);
    }
}
