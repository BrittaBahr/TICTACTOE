using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public interface IAnnouncementClientService
    {
        string Nickname { get; set; }

        void Start();

        void Stop();

        Task AcceptNicknameAsync(string nickname);

        IEnumerable<ITicTacToePlayer> GetPlayers();

        event EventHandler<GameRequestReceivedEventArgs> GameRequestReceived;

        event EventHandler<NewPlayerConnectedEventArgs> NewPlayerConnected;
    }
}
