namespace SharedLibrary
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    public interface ITicTacToePlayer
    {
        event EventHandler<GameTurnReceivedEventArgs> GameTurnReceived;

        event EventHandler<ReplayRequestReceivedEventArgs> ReplayRequestReceived;

        event EventHandler<AcknowledgeGameRequestReceivedEventArgs> GameRequestAcknowledgeReceived;

        event EventHandler<AcknowledgeGameTurnReceivedEventArgs> GameTurnAcknowledgeReceived;

        string Nickname { get; }

        IPEndPoint EndPoint { get; }

        Task RequestGameAsync(string ownNickname, IPAddress ownIPAddress);

        Task RequestGameTurnAsync(Position position);

        Task RequestReplayMessageAsync(string ownNickname, IPAddress ownIPAddress);

        void AcknowledgeGameRequest();

        void AcknowledgeGameTurnRequest();
    }
}
