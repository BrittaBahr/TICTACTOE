using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLibrary
{
    public class Position
    {
        private int xPosition;

        private int yPosition;

        public Position(int xPosition, int yPosition)
        {
            this.XPosition = xPosition;
            this.YPosition = yPosition;
        }

        public int XPosition
        {
            get
            {
                return this.xPosition;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.xPosition));
                }

                this.xPosition = value;
            }
        }

        public int YPosition
        {
            get
            {
                return this.yPosition;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(this.yPosition));
                }

                this.yPosition = value;
            }
        }
    }
}
