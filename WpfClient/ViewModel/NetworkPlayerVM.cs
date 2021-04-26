using Network;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TicTacToeGameLogic;
using WpfClient.ViewModel.Event;

namespace WpfClient.ViewModel
{
    public class NetworkPlayerVM : INotifyPropertyChanged
    {
        private readonly ITicTacToePlayer model;

        private readonly IWriter<string> informationWriter;

        private readonly IWriter<string> errorWriter;

        public NetworkPlayerVM(ITicTacToePlayer model)
        {
            this.model = model;
            this.model.GameRequestAcknowledgeReceived += ModelGameRequestAcknowledgeReceived;
            this.model.GameTurnReceived += ModelGameTurnReceived;
            this.model.GameTurnAcknowledgeReceived += ModelGameTurnAcknowledgeReceived;
            this.model.ReplayRequestReceived += ModelReplayRequestReceived;
            this.informationWriter = new InfoWriterView();
            this.errorWriter = new ErrorWriterView();
            string ownHostname = Environment.MachineName;
            IPAddress[] adresses = Dns.GetHostAddresses(ownHostname);
            this.OwnIPAddess = adresses[adresses.Length / 2];
        }

        public event EventHandler<NetworkPlayerGameStartedEventArgs>? GameStarted;

        public event EventHandler<NetworkPlayerGameTurnReceivedEventArgs>? GameTurnReceived;

        public event EventHandler<NetworkPlayerGameTurnAcknowledgeReceivedEventArgs>? GameTurnAcknowledgeReceived;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand GameRequestCommand
        {
            get
            {
                return new Command(async x => {
                    try
                    {
                        this.informationWriter.Write("Wait for answer.");
                        await this.model.RequestGameAsync(this.OwnNickname!, this.OwnIPAddess);
                    }
                    catch (RequestNotAcceptedException)
                    {
                        this.informationWriter.Write("Player won't play a game.");
                    }
                    catch (Exception)
                    {
                        this.errorWriter.Write("Something went wrong with the connection.");
                    }
                });
            }
        }

        public string? OwnNickname
        {
            get;
            set;
        }

        public IPAddress OwnIPAddess
        {
            get;
        }

        public string Nickname
        {
            get
            {
                return this.model.Nickname;
            }
        }

        public IPEndPoint IPEndPoint
        {
            get
            {
                return this.model.EndPoint;
            }
        }

        public void SendAknGameRequest()
        {
            this.model.AcknowledgeGameRequest();
        }

        public async Task RequestGameAsync(string ownNickname, IPAddress ownAddress)
        {
            await this.model.RequestGameAsync(ownNickname, ownAddress);
        }

        public async Task RequestGameTurnAsync(Position position)
        {
            await this.model.RequestGameTurnAsync(position);
        }

        public async Task RequestReplayMessageAsync(string ownNickname, IPAddress ownIPAddress)
        {
            await this.model.RequestReplayMessageAsync(ownNickname, ownIPAddress);
        }

        public void AcknowledgeGameRequest()
        {
            this.model.AcknowledgeGameRequest();
        }

        public void AcknowledgeGameTurnRequest()
        {
            this.model.AcknowledgeGameTurnRequest();
        }

        protected virtual void OnGameStarted(NetworkPlayerVM otherPlayerVM)
        {
            this.GameStarted?.Invoke(this, new NetworkPlayerGameStartedEventArgs(otherPlayerVM));
        }

        protected virtual void OnGameTurnReceived(Position position)
        {
            this.GameTurnReceived?.Invoke(this, new NetworkPlayerGameTurnReceivedEventArgs(position));
        }

        protected virtual void OnGameTurnAcknowledgeReceived()
        {
            this.GameTurnAcknowledgeReceived?.Invoke(this, new NetworkPlayerGameTurnAcknowledgeReceivedEventArgs());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ModelReplayRequestReceived(object? sender, ReplayRequestReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ModelGameTurnAcknowledgeReceived(object? sender, AcknowledgeGameTurnReceivedEventArgs e)
        {
            this.OnGameTurnAcknowledgeReceived();
        }

        private void ModelGameTurnReceived(object? sender, GameTurnReceivedEventArgs e)
        {
            this.OnGameTurnReceived(e.Position);
        }

        private void ModelGameRequestAcknowledgeReceived(object? sender, AcknowledgeGameRequestReceivedEventArgs e)
        {
            this.OnGameStarted(this);
        }
    }
}
