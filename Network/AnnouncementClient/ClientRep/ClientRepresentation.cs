using Network.EnhancedTcp;
using Network.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Network
{
    public class ClientRepresentation
    {
        private readonly TcpProtocol protocol;

        private byte[] buffer;

        public ClientRepresentation(EnhancedTcpClient client)
        {
            this.buffer = new byte[0];
            this.protocol = new TcpProtocol();
            this.protocol.GameRequestReceived += this.ProtocolGameRequestReceived;
            this.EnhancedTcpClient = client;
            this.EnhancedTcpClient.ConnectionClosed += this.EnhTcpClientConnectionClosed;
            this.EnhancedTcpClient.DataReceived += this.EnhTcpClientDataReceived;
        }

        public event EventHandler<ClientRepGameRequestReceivedEventArgs>? GameRequestReceived;

        public EnhancedTcpClient EnhancedTcpClient
        {
            get;
            private set;
        }

        protected virtual void OnGameRequestReceived(TcpPlayer tcpPlayer)
        {
            this.GameRequestReceived?.Invoke(this, new ClientRepGameRequestReceivedEventArgs(tcpPlayer));
        }

        private void ProtocolGameRequestReceived(object? sender, Protocol.Events.ProtocolGameRequestReceivedEventArgs e)
        {
            this.OnGameRequestReceived(new TcpPlayer(e.Nickname, this.EnhancedTcpClient));
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
