using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IConnectionService
    {
        public Task<Connection> AddConnection(Connection entity);
        
        public Task<Connection> GetConnection(int UserID);
        public Task DeleteOldConnection(int UserID);
    }
}
