using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation
{
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly FptclubsContext _context;

        public ConnectionRepository(FptclubsContext context)
        {
            _context = context;
        }

        public async Task<Connection> AddConnection(Connection entity)
        {
            await _context.Connections.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteConnection(Connection entity)
        {
            _context.Connections.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Connection>> GetConnections()
        {
            return await _context.Connections.ToListAsync();
        }
    }
}
