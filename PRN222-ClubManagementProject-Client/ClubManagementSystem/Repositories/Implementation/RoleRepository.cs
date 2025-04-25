using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly FptclubsContext _context;

        public RoleRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<Role> AddRoleAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<bool> DeleteRoleAsync(int roleID)
        {
            var role = await GetRoleAsync(roleID);
            _context.Roles.Remove(role);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<Role> GetRoleAsync(int roleID)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleID);
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            _context.Entry<Role>(role).State = EntityState.Modified;
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
