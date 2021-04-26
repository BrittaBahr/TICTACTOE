using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TicTacToeGameLogic;

namespace Network.Protocol.Events
{
    public class ProtocolGameRequestReceivedEventArgs : EventArgs
    {
        public ProtocolGameRequestReceivedEventArgs(string nickname, IPAddress iPAddress)
        {
            this.Nickname = nickname;
            this.IPAddress = iPAddress;
        }

        public string Nickname
        {
            get;
            private set;
        }

        public IPAddress IPAddress
        {
            get;
            private set;
        }
    }
}
