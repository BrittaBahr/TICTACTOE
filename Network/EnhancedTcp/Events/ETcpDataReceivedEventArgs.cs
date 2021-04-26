using System;
using System.Collections.Generic;
using System.Text;

namespace Network.EnhancedTcp.Events
{
    public class ETcpDataReceivedEventArgs : EventArgs
    {
        public ETcpDataReceivedEventArgs(EnhancedTcpClient sender, byte[] data)
        {
            this.Sender = sender;
            this.Data = data;
        }

        public EnhancedTcpClient Sender
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
