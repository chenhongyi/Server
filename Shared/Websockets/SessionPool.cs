﻿using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Websockets
{
    public class SessionPool
    {
        private static readonly SessionPool instance = new SessionPool();
        static SessionPool() { }
        private SessionPool() { }
        public static SessionPool Instance
        {
            get
            {
                return instance;
            }
        }
        static Dictionary<string, WebSocketSession> _sessions;
        Dictionary<string, WebSocketSession> sessions
        {
            get
            {
                if (_sessions == null)
                {
                    _sessions = new Dictionary<string, WebSocketSession>();
                }
                return _sessions;
            }
        }

        public void IPool(WebSocketSession session)
        {
            sessions.Add(session.SessionID, session);
        }

        public void OPool(WebSocketSession session)
        {
            sessions.Remove(session.SessionID);
        }

        public Dictionary<string, WebSocketSession> GetAll()
        {
            return sessions;
        }

        public WebSocketSession GetSid(string key)
        {

            WebSocketSession value = null;
            sessions.TryGetValue(key, out value);

            return value;
        }

    }
}
