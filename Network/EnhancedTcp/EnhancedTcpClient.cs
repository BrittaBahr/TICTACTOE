using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network.EnhancedNwStream;
using Network.EnhancedTcp.Events;
using SharedLibrary;

namespace Network.EnhancedTcp
{
    public class EnhancedTcpClient
    {
        private readonly TcpClient tcpClient;

        private EnhancedNetworkStream nwStream;

        private Thread? keepAliveThread;

        private KeepAliveThreadArgs? args;

        public EnhancedTcpClient(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            this.nwStream = new EnhancedNetworkStream(this.tcpClient.GetStream());
            this.nwStream.DataReceived += ENSDataReceived;
            this.nwStream.ConnectionClosed += ENSStreamConnectionClosed;
        }

        public event EventHandler<ETcpDataReceivedEventArgs>? DataReceived;

        public event EventHandler<ETcpConnectionClosedEventArgs>? ConnectionClosed;

        public bool IsRunning
        {
            get
            {
                return this.keepAliveThread != null && this.keepAliveThread.IsAlive;
            }
        }

        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException();
            }

            this.nwStream.Start();
            this.args = new KeepAliveThreadArgs();
            this.keepAliveThread = new Thread(this.Work);
            this.keepAliveThread.Start(this.args);
        }

        public void Stop()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException();
            }

            if (this.nwStream.IsRunning)
            {
                this.nwStream.Stop();
            }

            this.args!.Exit = true;

            if (Thread.CurrentThread != this.keepAliveThread)
            {
                this.keepAliveThread!.Join();
            }
        }

        public void Close()
        {
            if (this.IsRunning)
            {
                this.Stop();
            }

            this.nwStream.Close();
            this.tcpClient.Close();
        }

        public void Write(byte[] data)
        {
            this.nwStream.Write(data);
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            this.DataReceived?.Invoke(this, new ETcpDataReceivedEventArgs(this, data));
        }

        protected virtual void OnConnectionClosed()
        {
            this.ConnectionClosed?.Invoke(this, new ETcpConnectionClosedEventArgs(this));
        }

        private void ENSDataReceived(object? sender, EnhancedNwStream.Events.ENSDataReceivedEventArgs eventArgs)
        {
            this.OnDataReceived(eventArgs.Data);
        }

        private void ENSStreamConnectionClosed(object? sender, EnhancedNwStream.Events.ENSConnectionClosedEventArgs eventArgs)
        {
            this.OnConnectionClosed();
        }

        private void Work(object? threadArgsObj)
        {
            if (!(threadArgsObj is KeepAliveThreadArgs threadArgs))
            {
                throw new ArgumentException(nameof(threadArgsObj));
            }

            while (!threadArgs.Exit)
            {
                this.Write(new byte[] {0});
                Thread.Sleep(threadArgs.PollDelay);
            }
        }

        private class KeepAliveThreadArgs
        {
            private int pollDelay;

            public KeepAliveThreadArgs()
            {
                this.PollDelay = 50;
                this.Exit = false;
            }

            public int PollDelay
            {
                get
                {
                    return this.pollDelay;
                }

                set
                {
                    if (value < 0)
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
