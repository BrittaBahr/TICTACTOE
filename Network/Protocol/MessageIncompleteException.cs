using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Protocol
{
    public class MessageIncompleteException : Exception
    {
        public MessageIncompleteException() : base()
        {

        }
    }
}
