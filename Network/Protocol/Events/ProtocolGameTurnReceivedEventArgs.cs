using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeGameLogic;

namespace Network.Protocol.Events
{
    public class ProtocolGameTurnReceivedEventArgs : EventArgs
    {
        public ProtocolGameTurnReceivedEventArgs(Position position)
        {
            this.Position = position;
        }

        public Position Position
        {
            get;
            private set;
        }
    }
}
