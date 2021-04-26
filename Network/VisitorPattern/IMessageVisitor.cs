using Network.Messages;

namespace Network.VisitorPattern
{
    public interface IMessageVisitor
    {
        void Visit(NicknameMessage nicknameMessage);
    }

    public interface IMessageVisitor<T>
    {
        T Visit(NicknameMessage nicknameMessage);
    }
}