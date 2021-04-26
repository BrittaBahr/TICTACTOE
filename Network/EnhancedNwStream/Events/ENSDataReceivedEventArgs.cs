using System;
using System.Collections.Generic;
using System.Text;

namespace Network.EnhancedNwStream.Events
{
    public class ENSDataReceivedEventArgs : EventArgs
    {

        public ENSDataReceivedEventArgs(EnhancedNetworkStream sender, byte[] data)
        {
            this.Sender = sender;
            this.Data = data;
        }

        public EnhancedNetworkStream Sender
        {
            get;
            private set;
        }

        public byte[] Data
        {
            get;
            private set;
        }
    }
}
