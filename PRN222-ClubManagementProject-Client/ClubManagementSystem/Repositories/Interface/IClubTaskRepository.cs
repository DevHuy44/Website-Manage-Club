using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IClubTaskRepository
    {
        Task<IEnumerable<ClubTask>> GetClubTasksAsync();
        Task<ClubTask> GetClubTaskAsync(int ClubTaskID);
        Task<ClubTask> AddClubTaskAsync(ClubTask ClubTask);
        Task<Boolean> UpdateClubTaskAsync(ClubTask ClubTask);
        Task<Boolean> DeleteClubTaskAsync(int ClubTaskID);
    }
}
