using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public class GameTurnReceivedEventArgs : EventArgs
    {
        public GameTurnReceivedEventArgs(Position position)
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
