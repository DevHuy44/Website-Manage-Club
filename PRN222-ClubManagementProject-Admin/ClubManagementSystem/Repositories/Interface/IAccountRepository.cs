using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;

namespace Repositories.Interface
{
    public interface IAccountRepository
    {
        Task<User?> CheckLogin(string gmail, string password);
        Task<IEnumerable<ClubMember>> CheckRole(int userId);
        Task<User?> CheckEmailExist(string email);
        Task<User?> AddGmailUser(User user);
        Task<User?> AddUser(User user);
        Task<User?> CheckUsernameExist(string username);
        Task<User?> FindUserAsync(int userId);
        Task<User?> UpdateUserAsync(User user);
    }
}
