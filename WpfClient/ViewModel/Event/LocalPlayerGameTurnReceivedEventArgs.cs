using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeGameLogic;

namespace WpfClient.ViewModel.Event
{
    public class LocalPlayerGameTurnReceivedEventArgs : EventArgs
    {
        public LocalPlayerGameTurnReceivedEventArgs(Position position)
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
