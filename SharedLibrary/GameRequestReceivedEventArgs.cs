using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SharedLibrary
{
    public class GameRequestReceivedEventArgs : EventArgs
    {
        public GameRequestReceivedEventArgs(ITicTacToePlayer player)
        {
            this.Player = player;
        }

        public ITicTacToePlayer Player
        {
            get;
            private set;
        }
    }
}
