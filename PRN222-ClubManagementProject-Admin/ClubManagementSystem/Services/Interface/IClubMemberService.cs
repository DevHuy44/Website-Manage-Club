using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IClubMemberService
    {
        Task<ClubMember> AddClubMemberAsync(ClubMember clubMember);
        Task<ClubMember?> GetClubMemberAsync(int id);
        Task<IEnumerable<ClubMember>> GetClubMemberByUserId(int id);
        Task<ClubMember?> GetClubMemberAsync(int clubID, int userId);
        Task<bool> IsClubMember(int clubID, int userId);
        Task<IEnumerable<ClubMember>> GetClubMembersAsync(int clubId);
        Task<IEnumerable<ClubMember>> GetClubMembersAsync(int clubId, int roleID);
        Task<IEnumerable<ClubMember>> GetClubMembersAsync(int clubId, bool status);
        Task<(bool success, string message)> UpdateClubMemberAsync(ClubMember clubMember);
        Task<IEnumerable<ClubMember>> GetAllClubMembersByClubIdWithAnyStatusAsync(int clubId);
    }
}
