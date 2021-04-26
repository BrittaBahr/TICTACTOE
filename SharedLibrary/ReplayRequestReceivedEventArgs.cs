using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SharedLibrary
{
    public class ReplayRequestReceivedEventArgs : EventArgs
    {
        public ReplayRequestReceivedEventArgs(string nickname)
        {
            this.Nickname = nickname;
        }

        public string Nickname
        {
            get;
            private set;
        }
    }
}
