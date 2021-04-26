using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Network.EnhancedTcp;
using Network.Protocol;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeGameLogic;

namespace Network
{
    public class TcpPlayer : ITicTacToePlayer
    {
        private byte[] buffer;

        private EnhancedTcpClient? enhancedTcpClient;

        private TcpProtocol protocol;

        private bool boolGameRequestAnckowledgeReceived;

        private bool boolGameTurnAnckowledgeReceived;

        public TcpPlayer(string nickname, IPEndPoint endPoint)
        {
            this.Nickname = nickname;
            this.EndPoint = endPoint;
            this.boolGameRequestAnckowledgeReceived = false;
            this.boolGameTurnAnckowledgeReceived = false;
            this.protocol = new TcpProtocol();
            this.protocol.GameTurnReceived += this.ProtocolGameTurnReceived;
            this.protocol.ReplayRequestReceived += this.ProtocolReplayRequestReceived;
            this.protocol.AcknowledgeGameTurn += this.ProtocolAcknowledgeGameTurn;
            this.protocol.AcknowledgeGameRequest += this.ProtocolAcknowledgeGameRequest;
            this.buffer = new byte[0];
        }

        public TcpPlayer(string nickname, EnhancedTcpClient enhancedTcpClient)
        {
            this.enhancedTcpClient = enhancedTcpClient;
            enhancedTcpClient.DataReceived += this.EnhTcpClientDataReceived;
            enhancedTcpClient.ConnectionClosed += this.EnhTcpClientConnectionClosed;
            this.boolGameRequestAnckowledgeReceived = false;
            this.Nickname = nickname;
            this.protocol = new TcpProtocol();
            this.protocol.GameTurnReceived += this.ProtocolGameTurnReceived;
            this.protocol.ReplayRequestReceived += this.ProtocolReplayRequestReceived;
            this.protocol.AcknowledgeGameTurn += this.ProtocolAcknowledgeGameTurn;
            this.protocol.AcknowledgeGameRequest += this.ProtocolAcknowledgeGameRequest;
            this.buffer = new byte[0];
        }

        public event EventHandler<GameTurnReceivedEventArgs>? GameTurnReceived;

        public event EventHandler<ReplayRequestReceivedEventArgs>? ReplayRequestReceived;

        public event EventHandler<AcknowledgeGameRequestReceivedEventArgs>? GameRequestAcknowledgeReceived;

        public event EventHandler<AcknowledgeGameTurnReceivedEventArgs>? GameTurnAcknowledgeReceived;

        public IPEndPoint? EndPoint
        {
            get;
        }

        public string Nickname
        {
            get;
        }

        public async Task RequestGameAsync(string ownNickname, IPAddress ownIPAddress)
        {
            if (this.EndPoint == null)
            {
                throw new ArgumentException(nameof(this.EndPoint));
            }

            TcpClient client = new TcpClient();
            await client.ConnectAsync(this.EndPoint.Address, this.EndPoint.Port);
            this.enhancedTcpClient = new EnhancedTcpClient(client);
            this.enhancedTcpClient.ConnectionClosed += EnhTcpClientConnectionClosed;
            this.enhancedTcpClient.DataReceived += EnhTcpClientDataReceived;
            this.enhancedTcpClient.Start();
            this.enhancedTcpClient!.Write(this.protocol.ConvertGameRequest(ownNickname, ownIPAddress));
            double interval = 0;
            DateTime startTime = DateTime.Now;

            while (!this.boolGameRequestAnckowledgeReceived)
            {
                await Task.Delay(500);
                interval = (DateTime.Now - startTime).TotalMilliseconds;

                if (interval >= 10000)
                {
                    throw new RequestNotAcceptedException();
                }
            }
        }

        public async Task RequestGameTurnAsync(Position position)
        {
            if (this.enhancedTcpClient == null)
            {
                throw new InvalidOperationException();
            }

            this.enhancedTcpClient.Write(this.protocol.ConvertGameTurn(position));
            double interval = 0;
            DateTime startTime = DateTime.Now;

            while (!this.boolGameTurnAnckowledgeReceived)
            {
                await Task.Delay(50);
                interval = (DateTime.Now - startTime).TotalMilliseconds;

                if (interval > 10000)
                {
                    throw new RequestNotAcceptedException();
                }
            }
        }

        public async Task RequestReplayMessageAsync(string ownNickname, IPAddress ownIPAddress)
        {
            if (this.enhancedTcpClient == null)
            {
                throw new InvalidOperationException();
            }

            this.enhancedTcpClient.Write(this.protocol.ConvertReplayRequest(ownNickname, ownIPAddress));
            double interval = 0;
            DateTime startTime = DateTime.Now;

            while (!this.boolGameRequestAnckowledgeReceived)
            {
                await Task.Delay(50);
                interval = (DateTime.Now - startTime).TotalMilliseconds;

                if (interval < 10000)
                {
                    throw new RequestNotAcceptedException();
                }
            }
        }

        public void AcknowledgeGameRequest()
        {
            if (this.enhancedTcpClient == null)
            {
                throw new InvalidOperationException();
            }

            this.enhancedTcpClient.Write(this.protocol.ConvertAcknowledgeGameRequest());
        }

        public void AcknowledgeGameTurnRequest()
        {
            if (this.enhancedTcpClient == null)
            {
                throw new InvalidOperationException();
            }

            this.enhancedTcpClient.Write(this.protocol.ConvertAcknowledgeGameTurn());
        }

        protected virtual void OnGameTurnReceived(Position position)
        {
            this.GameTurnReceived?.Invoke(this, new GameTurnReceivedEventArgs(position));
        }

        protected virtual void OnReplayRequestReceived(string nickname)
        {
            this.ReplayRequestReceived?.Invoke(this, new ReplayRequestReceivedEventArgs(nickname));
        }

        protected virtual void OnGameTurnAcknowledgeReceived()
        {
            this.GameTurnAcknowledgeReceived?.Invoke(this, new AcknowledgeGameTurnReceivedEventArgs());
        }

        protected virtual void OnGameRequestAcknowledgeReceived()
        {
            this.GameRequestAcknowledgeReceived?.Invoke(this, new AcknowledgeGameRequestReceivedEventArgs());
        }

        private void ProtocolGameTurnReceived(object? sender, Protocol.Events.ProtocolGameTurnReceivedEventArgs e)
        {
            this.OnGameTurnReceived(e.Position);
        }

        private void ProtocolReplayRequestReceived(object? sender, Protocol.Events.ProtocolReplayRequestReceivedEventsArgs e)
        {
            this.OnReplayRequestReceived(this.Nickname);
        }

        private void ProtocolAcknowledgeGameRequest(object? sender, Protocol.Events.ProtocolAcknowledgeGameRequestEventArgs e)
        {
            this.boolGameRequestAnckowledgeReceived = true;
            this.OnGameRequestAcknowledgeReceived();
        }

        private void ProtocolAcknowledgeGameTurn(object? sender, Protocol.Events.ProtocolAcknowledgeGameTurnEventArgs e)
        {
            this.boolGameTurnAnckowledgeReceived = true;
            this.OnGameTurnAcknowledgeReceived();
        }

        private void EnhTcpClientDataReceived(object? sender, EnhancedTcp.Events.ETcpDataReceivedEventArgs e)
        {
            this.buffer = this.buffer.Concat(e.Data).ToArray();
            this.buffer = this.protocol.ParseMessages(this.buffer);
        }

        private void EnhTcpClientConnectionClosed(object? sender, EnhancedTcp.Events.ETcpConnectionClosedEventArgs e)
        {
            e.Sender.ConnectionClosed -= this.EnhTcpClientConnectionClosed;
            e.Sender.DataReceived -= this.EnhTcpClientDataReceived;
        }
    }
}
