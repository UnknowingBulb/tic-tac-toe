using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using XOX.Database;
using XOX.Models;

namespace XOX.BLObjects
{
    public class SessionListHandlerDb
    {
        private readonly SessionContext _context;
        public SessionListHandlerDb(SessionContext context)
        {
            _context = context;
        }

        public async Task<Session> AddSession(Session session)
        {
            SessionModel sessionModel = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == session.Id);
            if (sessionModel == null || session.Id == null)
            {
                sessionModel = new SessionModel(session);
                await _context.AddAsync(sessionModel);
                await _context.SaveChangesAsync();
                return sessionModel.toSession();
            }
            else
                return sessionModel.toSession();
        }

        public async Task<Session> GetSession(int sessionId)
        {
            SessionModel sessionModel = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            return sessionModel?.toSession();
        }
    }
}
