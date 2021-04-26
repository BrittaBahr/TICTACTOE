using System;
using System.Collections.Generic;
using System.Text;

namespace Network.EnhancedTcp.Events
{
    public class ETcpConnectionClosedEventArgs : EventArgs
    {
        public ETcpConnectionClosedEventArgs(EnhancedTcpClient sender)
        {
            this.Sender = sender;
        }

        public EnhancedTcpClient Sender
        {
            get;
            private set;
        }
    }
}
