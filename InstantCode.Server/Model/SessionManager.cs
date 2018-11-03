using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace InstantCode.Server.Model
{
    public class SessionManager
    {
        private static readonly Random Random = new Random();
        private static readonly IDictionary<int, Session> Sessions = new ConcurrentDictionary<int, Session>();

        public static Session Find(int id)
        {
            return Sessions[id];
        }

        public static Session CreateNew(string name, string[] participants)
        {
            var session = new Session
            {
                Id = NewId(),
                Name = name,
                Participants = participants
            };
            Sessions.Add(session.Id, session);
            return session;
        }

        private static int NewId()
        {
            var id = RandomId();
            while (Sessions.ContainsKey(id))
                id = RandomId();
            return id;
        }

        private static int RandomId()
        {
            var buf = new byte[4];
            Random.NextBytes(buf);
            return BitConverter.ToInt32(buf, 0);
        }
    }
}
