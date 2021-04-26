using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network.EnhancedNwStream.Events;

namespace Network.EnhancedNwStream
{
    public class EnhancedNetworkStream
    {
        private readonly NetworkStream nwStream;

        private Thread? listenerThread;

        private ListenerThreadArgs? args;

        public EnhancedNetworkStream(NetworkStream nwStream)
        {
            this.nwStream = nwStream;
        }

        public event EventHandler<ENSDataReceivedEventArgs>? DataReceived;

        public event EventHandler<ENSConnectionClosedEventArgs>? ConnectionClosed;

        public bool IsRunning
        {
            get
            {
                return this.listenerThread != null && this.listenerThread.IsAlive;
            }
        }

        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException();
            }

            this.args = new ListenerThreadArgs(this.nwStream);
            this.listenerThread = new Thread(this.Work);
            this.listenerThread.Start(this.args);
        }

        public void Stop()
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException();
            }

            this.args!.Exit = true;

            if (Thread.CurrentThread != this.listenerThread)
            {
                this.listenerThread!.Join();
            }
        }

        public void Close()
        {
            if (this.IsRunning)
            {
                this.Stop();
            }

            this.nwStream.Close();
        }

        public void Write(byte[] data)
        {
            try
            {
                this.nwStream.Write(data, 0, data.Length);
            }
            catch (Exception)
            {

                this.OnConnectionClosed();
            }
        }

        protected virtual void OnDataReceived(byte[] data)
        {
            this.DataReceived?.Invoke(this, new ENSDataReceivedEventArgs(this, data));
        }

        protected virtual void OnConnectionClosed()
        {
            this.ConnectionClosed?.Invoke(this, new ENSConnectionClosedEventArgs(this));
        }

        private void Work(object? threadArgsObj)
        {
            if (!(threadArgsObj is ListenerThreadArgs threadArgs))
            {
                throw new ArgumentException(nameof(threadArgsObj));
            }

            while (!threadArgs.Exit)
            {
                if (!threadArgs.NwStream.DataAvailable)
                {
                    Thread.Sleep(threadArgs.PollDelay);
                    continue;
                }

                byte[] receivedBuffer;
                int receivedByteCount; // counts the actual received bytes

                try
                {
                    receivedBuffer = new byte[threadArgs.BufferSize];
                    receivedByteCount = threadArgs.NwStream.Read(receivedBuffer, 0, receivedBuffer.Length);
                }
                catch (Exception)
                {
                    this.OnConnectionClosed();
                    continue;
                }

                this.OnDataReceived(receivedBuffer.Take(receivedByteCount).ToArray());
            }
        }

        private class ListenerThreadArgs
        {
            private int bufferSize;

            private int pollDelay;

            public ListenerThreadArgs(NetworkStream nwStream)
            {
                this.NwStream = nwStream;
                this.PollDelay = 50;
                this.BufferSize = 8192;
                this.Exit = false;
            }

            public NetworkStream NwStream
            {
                get;
                private set;
            }

            public int BufferSize
            {
                get
                {
                    return this.bufferSize;
                }

                private set
                {
                    if (value <= 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(this.bufferSize));
                    }

                    this.bufferSize = value;
                }
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

                    this.pollDelay = 0;
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
