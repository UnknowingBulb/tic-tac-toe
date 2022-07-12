using Microsoft.EntityFrameworkCore;
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
            }
            else
            {
                //TODO: do not change if same values
                sessionModel = sessionModel.ChangeWithSession(session);
                _context.Update(sessionModel);
                await _context.SaveChangesAsync();
            }
            return sessionModel.ToSession();
        }

        public async Task<Session> GetSession(int sessionId)
        {
            SessionModel sessionModel = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            return sessionModel?.ToSession();
        }
    }
}
