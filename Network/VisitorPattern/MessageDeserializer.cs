using System;
using System.Collections.Generic;
using System.Text;
using Network.Messages;
using Network.SerializerDeserializer;

namespace Network.VisitorPattern
{
    public class MessageDeserializer : IMessageVisitor<IMessage>
    {
        private byte[] buffer;

        public MessageDeserializer()
        {
            this.buffer = new byte[0];
        }

        public byte[] Buffer
        {
            get
            {
                return this.buffer;
            }

            set
            {
                this.buffer = value ?? throw new ArgumentNullException("It can not be null.");
            }
        }

        public IMessage Visit(NicknameMessage nicknameMessage)
        {
            if (this.Buffer.Length == 0)
            {
                throw new ArgumentException("The buffer can not be null.");
            }

            return SerializerDeserializer<NicknameMessage>.Deserialize(this.Buffer);
        }
    }
}
