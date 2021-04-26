using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Protocol
{
    public class ParseObject
    {
        private int index;

        public ParseObject(byte[] receivedBuffer)
        {
            this.ReceivedBuffer = receivedBuffer;
            this.Index = 0;
        }

        public int Index
        {
            get
            {
                return this.index;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.index));
                }

                this.index = value;
            }
        }

        public byte[] ReceivedBuffer
        {
            get;
            private set;
        }
    }
}
