using BussinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IConnectionRepository
    {
        public Task<Connection> AddConnection(Connection entity);

        public Task<IEnumerable<Connection>> GetConnections();

        public Task DeleteConnection(Connection entity);
    }
}
