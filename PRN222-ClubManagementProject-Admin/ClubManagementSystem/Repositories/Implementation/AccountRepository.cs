using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;

namespace Repositories.Implementation
{
    public class AccountRepository : IAccountRepository
    {
        private readonly FptclubsContext _context;
        public AccountRepository(FptclubsContext context)
        {
            _context = context;
        }
        public async Task<User?> CheckLogin(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                return user;
            }
            return null;
        }
        public async Task<IEnumerable<ClubMember>> CheckRole(int userId)
        {
            var clubMembers = await _context.ClubMembers
                .Include(c => c.Role)
                .Include(c => c.Club)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            return clubMembers;
        }
        public async Task<User?> CheckEmailExist(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Email == email);
            if (user != null)
            {
                return user;
            }
            return null;
        }
        public async Task<User> AddGmailUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> CheckUsernameExist(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Username == username);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<User?> FindUserAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

    }
}
