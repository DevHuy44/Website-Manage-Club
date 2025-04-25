using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IClubMemberRepository
    {
        public Task<ClubMember?> GetClubMemberAsync(int id);
        public Task<ClubMember?> GetClubMemberAsync(int id, int clubId);
        public Task<IEnumerable<ClubMember>> GetClubMembersAsync();
        public Task<ClubMember> AddClubMemberAsync(ClubMember clubMember);
        public Task DeleteClubMemberAsync(int id);
        public Task UpdateClubMemberAsync(ClubMember clubMember);
        public Task<IEnumerable<ClubMember>> GetAllClubMembersByClubIdWithAnyStatusAsync(int clubId);
    }
}
