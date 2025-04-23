using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IClubTaskService
    {
        Task<ClubTask> AddClubTaskAsync(ClubTask task);
        Task<IEnumerable<ClubTask>> GetClubTasksAsync();
        Task<IEnumerable<ClubTask>> GetClubTasksAsync(int clubID);
        Task<IEnumerable<ClubTask>> GetPersonalClubTasksAsync(int userID);
        Task<IEnumerable<ClubTask>> GetClubTasksAsync(int clubID, int eventID);
        Task<ClubTask> GetClubTask(int taskID);
        Task<bool> UpdateClubTaskAsync(ClubTask task);
        Task<bool> IsAssigned(int taskID);
        Task<bool> IsCompleted(int taskID);
    }
}
