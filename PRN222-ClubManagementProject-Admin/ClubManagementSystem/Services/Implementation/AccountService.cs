using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessObjects.Models;
using BussinessObjects.Models.Dtos;
using Repositories.Interface;
using Services.Interface;

namespace Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task<User?> CheckLogin(string gmail, string password)
        {
            return _accountRepository.CheckLogin(gmail, password);
        }
        public Task<IEnumerable<ClubMember>> CheckRole(int userId)
        {
            return _accountRepository.CheckRole(userId);
        }
        public Task<User?> CheckEmailExist(string email)
        {
            return _accountRepository.CheckEmailExist(email);
        }
        public Task<User> AddGmailUser (User user)
        {
            return _accountRepository.AddGmailUser(user);
        }
        public Task<User> AddUser(User user)
        {
            return _accountRepository.AddUser(user);
        }

        public Task<User?> CheckUsernameExist(string username)
        {
            return _accountRepository.CheckUsernameExist(username);
        }

        public Task<User?> FindUserAsync(int id)
        {
            return _accountRepository.FindUserAsync(id);
        }

        public Task<User?> UpdateUserAsync(User user)
        {
            return _accountRepository.UpdateUserAsync(user);
        }
        public async Task<(bool Success, string Message)> UpdateUserProfileAsync(int userId, EditUserDto editUser)
        {
            var user = await _accountRepository.FindUserAsync(userId);
            if (user == null)
            {
                return (false, "User not found.");
            }

            if (!string.IsNullOrEmpty(editUser.Username) && user.Username != editUser.Username)
            {
                var usernameValid = await CheckUsernameExist(editUser.Username);
                if (usernameValid != null)
                {
                    return (false, "This username is already registered.");
                }
                user.Username = editUser.Username;
            }

            if (!string.IsNullOrEmpty(editUser.NewPassword))
            {
                if (editUser.NewPassword != editUser.ConfirmNewPassword)
                {
                    return (false, "Passwords do not match.");
                }
                user.Password = editUser.NewPassword; 
            }

            await _accountRepository.UpdateUserAsync(user);
            return (true, "Profile updated successfully.");
        }

        public async Task<byte[]> GetDefaultProfilePictureAsync()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "defaultPicture.jpg");

            if (!System.IO.File.Exists(filePath))
            {
                return Array.Empty<byte>(); 
            }

            return await System.IO.File.ReadAllBytesAsync(filePath);
        }


    }
}
