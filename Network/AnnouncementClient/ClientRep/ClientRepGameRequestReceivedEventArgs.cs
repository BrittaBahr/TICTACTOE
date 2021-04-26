using Network.EnhancedTcp;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Network
{
    public class ClientRepGameRequestReceivedEventArgs : EventArgs
    {
        public ClientRepGameRequestReceivedEventArgs(ITicTacToePlayer ticTacToePlayer)
        {
            this.TicTacToePlayer = ticTacToePlayer;
        }

        public ITicTacToePlayer TicTacToePlayer
        {
            get;
            private set;
        }
    }
}
