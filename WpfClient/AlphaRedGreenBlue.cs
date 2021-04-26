using System;
using System.Collections.Generic;
using System.Text;

namespace WpfClient
{
    public struct AlphaRedGreenBlue
    {
        public AlphaRedGreenBlue(byte alpha, byte red, byte green, byte blue)
        {
            this.Alpha = alpha;
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }

        public byte Red
        {
            get;
            set;
        }

        public byte Green
        {
            get;
            set;
        }

        public byte Blue
        {
            get;
            set;
        }

        public byte Alpha
        {
            get;
            set;
        }
    }
}
