using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Input;
using Network.Messages;
using Network.SerializerDeserializer;
using Network.Tuples;
using Network.VisitorPattern;
using System.Threading;
using Network.EnhancedTcp;
using Microsoft.Extensions.Logging;

namespace Network
{
    public class NetworkAnnouncementClientService : IAnnouncementClientService, IMessageVisitor
    {
        private readonly TcpListener tcpListener;

        private Thread? listenerThread;

        private ListenerThreadArgs? args;

        private readonly MessageIDVisitor messageIDVisitor;

        private readonly MessageDeserializer deserializer;

        private readonly List<TcpPlayer> players;

        public NetworkAnnouncementClientService(int port)
        {
            this.messageIDVisitor = new MessageIDVisitor();
            this.deserializer = new MessageDeserializer();
            this.Port = port;
            this.players = new List<TcpPlayer>();
            this.Messages = new List<IMessage>() { new NicknameMessage("--") };
            this.tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public event EventHandler<GameRequestReceivedEventArgs>? GameRequestReceived;

        public event EventHandler<NewPlayerConnectedEventArgs>? NewPlayerConnected;

        public bool IsRunning
        {
            get
            {
                return this.listenerThread != null && this.listenerThread.IsAlive;
            }
        }

        public int Port { get; set; }

        public string? Nickname { get; set; }

        public bool IsReceiving { get; set; }

        public List<IMessage> Messages
        {
            get;
            set;
        }

        public Action<IMessage>? HandleMessageReceived
        {
            get;
            set;
        }

        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException();
            }

            this.tcpListener.Start();
            this.args = new ListenerThreadArgs(this.tcpListener);
            this.listenerThread = new Thread(this.Work);
            this.listenerThread.Start(this.args);
        }

        public void Stop()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException();
            }

            this.tcpListener.Stop();
            this.args!.Exit = true;

            if (Thread.CurrentThread != this.listenerThread)
            {
                this.listenerThread!.Join();
            }
        }

        public async Task SendNicknameMessageAsync(string nickname)
        {
            NicknameMessage message = new NicknameMessage(nickname);
            byte[] buffer = BitConverter.GetBytes(this.messageIDVisitor.Visit(message));
            buffer = buffer.Concat(SerializerDeserializer<NicknameMessage>.Serialize(message)).ToArray();
            while (this.IsReceiving)
            {
                await this.SendMessagesAsync(buffer);
                await Task.Delay(1000);
            }
        }

        public async Task ReceiveAsync()
        {
            while (this.IsReceiving)
            {
                Task<ReceivedDataTuple> task = this.ReceiveMessageAsync();
                await Task.WhenAll(task);
                if (task.Result != null)
                {
                    byte[] buffer = task.Result.Buffer;
                    byte[] numberMessage = buffer.Take(4).ToArray();
                    buffer = buffer.Skip(4).ToArray();
                    if (buffer.Length != 0)
                    {
                        int number = BitConverter.ToInt32(numberMessage, 0);
                        var messageType = this.Messages.Where(type => type.Accept(messageIDVisitor) == number);
                        foreach (IMessage item in messageType)
                        {
                            this.deserializer.Buffer = buffer;
                            IMessage messageWithContent = item.Accept(this.deserializer);
                            messageWithContent.Endpoint = task.Result.Endpoint;
                            this.HandleMessageReceived?.Invoke(messageWithContent);
                        }
                    }
                }

                //await Task.Delay(1000);
            }
        }

        public async Task SendMessagesAsync(byte[] datagram)
        {
            while (true)
            {
                using (var udpClient = new UdpClient() { EnableBroadcast = true })
                {
                    udpClient.Client.ReceiveTimeout = 1000;  // 1 sec timeout
                    var endpoint = new IPEndPoint(IPAddress.Broadcast, this.Port);
                    await udpClient.SendAsync(datagram, datagram.Length, endpoint).ConfigureAwait(false);
                }

                break;
            }
        }

        public IEnumerable<ITicTacToePlayer> GetPlayers()
        {
            return this.players;
        }

        public async Task AcceptNicknameAsync(string nickname)
        {
            try
            {
                this.IsReceiving = true;
                this.HandleMessageReceived = x => x.Accept(this);
                Task sendName = this.SendNicknameMessageAsync(nickname);
                Task receive = this.ReceiveAsync();
                await Task.WhenAll(receive, sendName);
            }
            catch (Exception)
            {
                // TODO ILogger
                //this.ErrorWriter.Write("Something went wrong :/ Please close any difficult processes.");
            }
        }

        public void Visit(NicknameMessage nicknameMessage)
        {
            var allValidPLayer = this.players.Where(x => x.Nickname == nicknameMessage.Nickname && x.EndPoint!.Address.Equals(nicknameMessage.Endpoint.Address));
            if (allValidPLayer.Count() == 0)
            {
                // TODO: check for local IP adress(es!)
                if (nicknameMessage.Nickname != this.Nickname && nicknameMessage.Endpoint.Address != IPAddress.Any)
                {
                    TcpPlayer player = new TcpPlayer(nicknameMessage.Nickname, new IPEndPoint(nicknameMessage.Endpoint.Address, this.Port));
                    this.players.Add(player);
                    this.OnNewPlayerConnected(player);
                }
            }
        }

        protected virtual void OnPlayerRequestedGame(ITicTacToePlayer player)
        {
            this.GameRequestReceived?.Invoke(this, new GameRequestReceivedEventArgs(player));
        }

        protected virtual void OnNewPlayerConnected(TcpPlayer player)
        {
            this.NewPlayerConnected?.Invoke(this, new NewPlayerConnectedEventArgs(player));
        }

        private async Task<ReceivedDataTuple> ReceiveMessageAsync()
        {

            var endpoint = new IPEndPoint(IPAddress.Broadcast, this.Port);
            UdpReceiveResult result;
            ReceivedDataTuple tuple;
            using (var udpClient = new System.Net.Sockets.UdpClient(endpoint.Port) { EnableBroadcast = true })
            {
                try
                {
                    result = await udpClient.ReceiveAsync().ConfigureAwait(false);
                    tuple = new ReceivedDataTuple(result.Buffer, result.RemoteEndPoint);
                }
                catch (Exception)
                {
                    tuple = new ReceivedDataTuple(new byte[0], endpoint);
                }
            }


            return tuple;
        }

        private void Work(object? threadArgsObj)
        {
            if (!(threadArgsObj is ListenerThreadArgs threadArgs))
            {
                throw new InvalidOperationException();
            }

            while (!threadArgs.Exit)
            {
                if (threadArgs.TcpListener.Pending())
                {
                    TcpClient tcpClient = threadArgs.TcpListener.AcceptTcpClient();
                    EnhancedTcpClient enhTcpClient = new EnhancedTcpClient(tcpClient);
                    ClientRepresentation clientRep = new ClientRepresentation(enhTcpClient);
                    clientRep.GameRequestReceived += this.ClientRepGameRequestReceived;
                    enhTcpClient.Start();
                }
                else
                {
                    Thread.Sleep(threadArgs.PollDelay);
                }
            }
        }

        private void ClientRepGameRequestReceived(object? sender, ClientRepGameRequestReceivedEventArgs e)
        {
            this.OnPlayerRequestedGame(e.TicTacToePlayer);
        }

        private class ListenerThreadArgs
        {
            private int pollDelay;

            public ListenerThreadArgs(TcpListener tcpListener)
            {
                this.TcpListener = tcpListener;
                this.PollDelay = 50;
                this.Exit = false;
            }

            public TcpListener TcpListener
            {
                get;
                private set;
            }

            public int PollDelay
            {
                get
                {
                    return this.pollDelay;
                }

                set
                {
                    if (value <= 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(this.pollDelay));
                    }

                    this.pollDelay = value;
                }
            }

            public bool Exit
            {
                get;
                set;
            }
        }
    }
}
