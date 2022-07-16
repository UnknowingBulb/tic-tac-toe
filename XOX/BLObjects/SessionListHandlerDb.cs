using FluentResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using XOX.Database;
using XOX.Models;

namespace XOX.BLObjects
{
    public static class SessionListHandlerDb
    {
        public static async Task<Result<Session>> AddSession(Session session)
        {
            var contextOptions = new DbContextOptionsBuilder<SessionContext>().Options;
            using (var context = new SessionContext(contextOptions))
            {
                SessionModel sessionModel = await context.Sessions.Include(c => c.UserSessions).SingleOrDefaultAsync(s => s.Id == session.Id);
                if (sessionModel == null || session.Id == null)
                {
                    sessionModel = new SessionModel(session);
                    using var transaction = context.Database.BeginTransaction();
                    await context.AddAsync(sessionModel);
                    await context.SaveChangesAsync();
                    //by current logic we ALWAYS go into below if but i'll still leave it here just in case
                    if (session.Player1 != null)
                    {
                        await context.AddAsync(new UserSessionsModel()
                        {
                            SessionModelId = sessionModel.Id,
                            UserModelId = session.Player1.Id,
                            IsActive = session.IsActivePlayer1
                        });
                    }
                    //by current logic we NEVER go into below if but i'll still leave it here just in case
                    if (session.Player2 != null)
                    {
                        await context.AddAsync(new UserSessionsModel()
                        {
                            SessionModelId = sessionModel.Id,
                            UserModelId = session.Player2.Id,
                            IsActive = !session.IsActivePlayer1
                        });
                    }
                    await context.SaveChangesAsync();
                    transaction.Commit();
                }
                else if (!session.IsEqualByData(sessionModel))
                {
                    if (sessionModel.UserSessions.Count == 0)
                    {
                        if (session.Player1 != null)
                            await context.AddAsync(new UserSessionsModel(session.Player1.Id, sessionModel.Id, session.IsActivePlayer1));

                        if (session.Player2 != null)
                            await context.AddAsync(new UserSessionsModel(session.Player2.Id, sessionModel.Id, !session.IsActivePlayer1));
                    }
                    else if (sessionModel.UserSessions.Count == 1)
                    {
                        var userSession = sessionModel.UserSessions.ElementAt(0);
                        if (session.Player1 != null && userSession.UserModelId == session.Player1.Id)
                        {
                            userSession.IsActive = session.IsActivePlayer1;
                            context.Update(userSession);

                            if (session.Player2 != null && session.Player2.Id != Guid.Empty)
                                await context.AddAsync(new UserSessionsModel(session.Player2.Id, sessionModel.Id, !session.IsActivePlayer1));
                        }
                        else if (session.Player2 != null && userSession.UserModelId == session.Player2.Id)
                        {
                            userSession.IsActive = !session.IsActivePlayer1;
                            context.Update(userSession);

                            //probably we never go into the following if-statement
                            if (session.Player1 != null)
                                await context.AddAsync(new UserSessionsModel(session.Player1.Id, sessionModel.Id, session.IsActivePlayer1));
                        }
                        else
                        {
                            return Result.Fail("Problems with users in this session");
                        }
                    }
                    else if (sessionModel.UserSessions.Count == 2)
                    {
                        //we don't remove users from session so this is something exceptional
                        if (session.Player1 == null || session.Player2 == null || session.Player1.Id == Guid.Empty || session.Player2.Id == Guid.Empty)
                            return Result.Fail("Problems with users in this session");
                        var userSession1 = sessionModel.UserSessions.ElementAt(0);
                        var userSession2 = sessionModel.UserSessions.ElementAt(1);
                        if (userSession1.UserModelId == session.Player1.Id && userSession2.UserModelId == session.Player2.Id)
                        {
                            userSession1.IsActive = session.IsActivePlayer1;
                            userSession2.IsActive = !session.IsActivePlayer1;
                        }
                        //if we read from database in other order
                        else if (userSession1.UserModelId == session.Player2.Id && userSession2.UserModelId == session.Player1.Id)
                        {
                            userSession1.IsActive = !session.IsActivePlayer1;
                            userSession2.IsActive = session.IsActivePlayer1;
                        }
                        else {
                            return Result.Fail("Problems with users in this session");
                        }

                        context.Update(userSession1);
                        context.Update(userSession2);
                    }
                    else
                    {
                        return Result.Fail("Problems with users in this session");
                    }

                    sessionModel = session.ChangeModel(sessionModel);
                    context.Update(sessionModel);
                    await context.SaveChangesAsync();
                }
                session.Id = sessionModel.Id;

                return session;
            }
        }

        public static async Task<Result<Session>> GetSession(int? sessionId)
        {
            var contextOptions = new DbContextOptionsBuilder<SessionContext>().Options;
            using (var context = new SessionContext(contextOptions))
            {
                var sessionModel = await context.Sessions
                .Include(c => c.UserSessions)
                    .ThenInclude(i => i.User)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

                if (sessionModel == null || sessionModel.Id == null)
                    return Result.Fail("Game session not found");

                return new Session(sessionModel.Id,
                    sessionModel.UserSessions.ElementAtOrDefault(0)?.User,
                    sessionModel.UserSessions.ElementAtOrDefault(1)?.User,
                    sessionModel.Field, 
                    sessionModel.State,
                    sessionModel.UserSessions.ElementAtOrDefault(0)?.IsActive);
            }
        }
    }
}
