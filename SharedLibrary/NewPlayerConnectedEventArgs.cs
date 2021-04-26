using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public class NewPlayerConnectedEventArgs : EventArgs
    {
        public NewPlayerConnectedEventArgs(ITicTacToePlayer player)
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
