using Network;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Input;
using WpfClient.ViewModel.Event;

namespace WpfClient.ViewModel
{
    public class NetworkAnnouncementClientServiceVM : INotifyPropertyChanged
    {
        private readonly IAnnouncementClientService model;

        private readonly IWriter<string> errorWriter;

        public NetworkAnnouncementClientServiceVM(IAnnouncementClientService model)
        {
            this.errorWriter = new ErrorWriterView();
            this.model = model;
            this.model.NewPlayerConnected += ModelNewPlayerConnected;
            this.model.GameRequestReceived += ModelPlayerRequestedGame;
            this.StartListening();
            this.Players = new ObservableCollection<NetworkPlayerVM>();

            foreach (TcpPlayer player in this.model.GetPlayers())
            {
                NetworkPlayerVM playerVM = new NetworkPlayerVM(player);
                playerVM.OwnNickname = this.Nickname;
                playerVM.GameStarted += this.PlayerVMGameStarted;
                this.Players.Add(playerVM);
            }
        }

        public event EventHandler<NwAnnouncementClientServicePlayerRequestedGameEventArgs>? PlayerRequestedGame;

        public event EventHandler<NwAnnouncementClientServiceGameStartedEventArg>? GameStarted;

        public event EventHandler<ValidNicknameReceivedEventArgs>? ValidNicknameReceived;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Nickname
        {
            get
            {
                return this.model.Nickname;
            }

            set
            {
                if (value == string.Empty)
                {
                    throw new ArgumentException("The nickname can not be empty. Please you have to enter a nickname.");
                }

                if (this.model.Nickname != value)
                {
                    this.model.Nickname = value;
                    this.OnPropertyChanged(nameof(this.Nickname));
                }
            }
        }

        public ObservableCollection<NetworkPlayerVM> Players
        {
            get;
        }

        public ICommand ConfirmNickname
        {
            get
            {
                return new Command(async x =>
                {
                    if (string.IsNullOrEmpty(this.Nickname) || string.IsNullOrWhiteSpace(this.Nickname))
                    {
                        this.errorWriter.Write("This is not a valid nickname. The nickname cannot be empty or white space.");
                    }
                    else
                    {
                        this.OnValidNicknameReceived();
                        await this.model.AcceptNicknameAsync(this.Nickname);
                    }
                });
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnValidNicknameReceived()
        {
            this.ValidNicknameReceived?.Invoke(this, new ValidNicknameReceivedEventArgs());
        }

        protected virtual void OnGameStarted(NetworkPlayerVM otherPlayerVM)
        {
            this.GameStarted?.Invoke(this, new NwAnnouncementClientServiceGameStartedEventArg(otherPlayerVM));
        }

        protected virtual void OnPlayerRequestedGame(NetworkPlayerVM playerVM)
        {
            this.PlayerRequestedGame?.Invoke(this, new NwAnnouncementClientServicePlayerRequestedGameEventArgs(playerVM));
        }

        private void ModelPlayerRequestedGame(object? sender, SharedLibrary.GameRequestReceivedEventArgs e)
        {
            this.OnPlayerRequestedGame(new NetworkPlayerVM(e.Player));
        }

        private void ModelNewPlayerConnected(object? sender, NewPlayerConnectedEventArgs e)
        {
            TcpPlayer player = new TcpPlayer(e.Player.Nickname, e.Player.EndPoint);
            NetworkPlayerVM playerVM = new NetworkPlayerVM(player);
            playerVM.OwnNickname = this.Nickname;
            playerVM.GameStarted += PlayerVMGameStarted;
            this.Players.Add(playerVM);
        }

        private void PlayerVMGameStarted(object? sender, NetworkPlayerGameStartedEventArgs e)
        {
            this.OnGameStarted(e.OtherPlayerVM);
        }

        private void StartListening()
        {
            try
            {
                this.model.Start();
            }
            catch (Exception)
            {
                this.errorWriter.Write("An error occured by starting the listener.");
            }
        }
    }
}
