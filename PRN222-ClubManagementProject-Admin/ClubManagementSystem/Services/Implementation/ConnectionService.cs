using BussinessObjects.Models;
using Repositories.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConnectionRepository _CR;

        public ConnectionService(IConnectionRepository CR)
        {
            _CR = CR;
        }

        public async Task<Connection> AddConnection(Connection entity)
        {
            return await _CR.AddConnection(entity);
        }

        public async Task DeleteOldConnection(int UserID)
        {
            var connections = (await _CR.GetConnections()).Where(c => c.UserId == UserID);
            foreach (var connection in connections)
            {
                await _CR.DeleteConnection(connection);
            }
        }

        public async Task<Connection> GetConnection(int UserID)
        {
            return (await _CR.GetConnections()).Where(c => c.UserId == UserID).OrderByDescending(c => c.connectAt).FirstOrDefault();
        }
    }
}
