using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Network.VisitorPattern;

namespace Network.Messages
{
    public interface  IMessage
    {
        void Accept(IMessageVisitor visitor);

        T Accept<T>(IMessageVisitor<T> visitor);

        IPEndPoint Endpoint { get; set; }
    }
}
