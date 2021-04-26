using System;
using System.Collections.Generic;
using System.Text;
using Network.Messages;

namespace Network.VisitorPattern
{
    public class MessageIDVisitor : IMessageVisitor<int>
    {
        public int Visit(NicknameMessage nicknameMessage)
        {
            return 1;
        }
    }
}
