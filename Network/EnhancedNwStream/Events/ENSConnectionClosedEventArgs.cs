using System;
using System.Collections.Generic;
using System.Text;

namespace Network.EnhancedNwStream.Events
{
    public class ENSConnectionClosedEventArgs : EventArgs
    {
        public ENSConnectionClosedEventArgs(EnhancedNetworkStream sender)
        {
            this.Sender = sender;
        }

        public EnhancedNetworkStream Sender
        {
            get;
            private set;
        }


    }
}
