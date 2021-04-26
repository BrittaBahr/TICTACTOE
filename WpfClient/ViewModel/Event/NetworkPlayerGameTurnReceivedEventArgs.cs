using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeGameLogic;

namespace WpfClient.ViewModel.Event
{
    public class NetworkPlayerGameTurnReceivedEventArgs : EventArgs
    {
        public NetworkPlayerGameTurnReceivedEventArgs(Position position)
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
