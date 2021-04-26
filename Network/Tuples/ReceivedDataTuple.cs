using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Network.Tuples
{
    public class ReceivedDataTuple
    {
        public ReceivedDataTuple(byte[] buffer, IPEndPoint endPoint)
        {
            this.Buffer = buffer;
            this.Endpoint = endPoint;
        }

        public byte[] Buffer
        {
            get;
            set;
        }

        public IPEndPoint Endpoint
        {
            get;
            set;
        }
    }
}
