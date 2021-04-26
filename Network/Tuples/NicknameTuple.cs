using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace Network.Tuples
{
    public class NicknameTuple
    {
        public NicknameTuple(string nickname, IPEndPoint endpoint, Action sendRequestMessage)
        {
            this.Nickname = nickname;
            this.Endpoint = endpoint;
            this.SendRequestMessage = sendRequestMessage;
        }

        public string Nickname
        {
            get;
            set;
        }

        public IPEndPoint Endpoint
        {
            get;
            set;
        }

        public Action SendRequestMessage
        {
            get;
            set;
        }
    }
}
