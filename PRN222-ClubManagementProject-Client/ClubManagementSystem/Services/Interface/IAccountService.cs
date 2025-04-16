using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAccountService
    {
        Task<User?> CheckLogin(string username, string password);
        Task<IEnumerable<ClubMember>> CheckRole(int userId);
        Task<User?> CheckEmailExist(string email);
        Task<User> AddGmailUser(User user);
        Task<User> AddUser(User user);
        Task<User?> CheckUsernameExist(string username);
        Task<User?> FindUserAsync(int userId);
        Task<User?> UpdateUserAsync(User user);
        Task<(bool Success, string Message)> UpdateUserProfileAsync(int userId, EditUserDto editUser);
        Task<byte[]> GetDefaultProfilePictureAsync();
    }

}
