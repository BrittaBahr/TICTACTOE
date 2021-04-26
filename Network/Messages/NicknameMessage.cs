using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Network.VisitorPattern;

namespace Network.Messages
{
    [Serializable]
    public class NicknameMessage : IMessage
    {
        [NonSerialized]
        private IPEndPoint endpoint;

        public NicknameMessage(string nickname)
        {
            this.Nickname = nickname;
            this.endpoint = new IPEndPoint(IPAddress.Loopback,80); // default value
        }

        public string Nickname
        {
            get;
            set;
        }

        public IPEndPoint Endpoint
        {
            get
            {
                return this.endpoint;
            }

            set
            {
                this.endpoint = value ?? throw new ArgumentNullException("It can not be null.");
            }
        }

        public void Accept(IMessageVisitor visitor)
        {
            visitor.Visit(this);
        }

        public T Accept<T>(IMessageVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
