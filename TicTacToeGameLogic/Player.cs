using SharedLibrary;
using System;
using System.Net;
using System.Threading.Tasks;

namespace TicTacToeGameLogic
{
    public class Player : ITicTacToePlayer
    {
        public Player(int width, int height, string name)
        {
            if (width <= 0 || height <= 0)
            {
                throw new ArgumentOutOfRangeException("At least one of the parameters is negative or 0.");
            }

            this.Field = new Player[width, height];
            this.Nickname = name;
            this.SetField();
            this.Won = false;
            this.Lost = false;
        }

        public string Nickname
        {
            get;
        }

        public IPEndPoint? EndPoint
        {
            get;
        }

        public bool Won
        {
            get;
            private set;
        }

        public bool Lost
        {
            get;
            private set;
        }

        public Player?[,] Field
        {
            get;
            private set;
        }

        public event EventHandler<GameTurnReceivedEventArgs>? GameTurnReceived;

        public event EventHandler<ReplayRequestReceivedEventArgs>? ReplayRequestReceived;

        public event EventHandler<AcknowledgeGameRequestReceivedEventArgs>? GameRequestAcknowledgeReceived;

        public event EventHandler<AcknowledgeGameTurnReceivedEventArgs>? GameTurnAcknowledgeReceived;

        public void MakeGameTurn(Position position)
        {
            // check for valid parameter
            if (position.XPosition >= this.Field.GetLength(0) || position.YPosition >= this.Field.GetLength(1))
            {
                throw new ArgumentOutOfRangeException($"The {nameof(position)} is not allowed to be greater than the field length.");
            }

            if (this.Field[position.XPosition, position.YPosition] != null)
            {
                throw new InvalidOperationException("It is not allowed to place a player on a field, which is not null.");
            }

            this.Field[position.XPosition, position.YPosition] = this;
            this.Won = this.CheckForHorizontalWinner(this) || this.CheckForVerticalWinner(this) || this.CheckForDiagonalWinner(this) || this.CheckForDiagonalDownWinner(this);

            this.OnGameTurnReceived(position);
        }

        public void AcknowledgeGameRequest()
        {
            this.OnGameRequestAcknowledgeReceived();
        }

        public void AcknowledgeGameTurnRequest()
        {
            this.OnGameTurnAcknowledgeReceived();
        }

        public Task RequestGameAsync(string ownNickname, IPAddress ownIPAddress)
        {
            return Task.CompletedTask;
        }

        public Task RequestGameTurnAsync(Position position)
        {
            return Task.CompletedTask;
        }

        public Task RequestReplayMessageAsync(string ownNickname, IPAddress ownIPAddress)
        {
            return Task.CompletedTask;
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

        private void SetField()
        {
            // go throw columns
            for (int i = 0; i < this.Field.GetLength(0); i++)
            {
                // go throw rows
                for (int j = 0; j < this.Field.GetLength(1); j++)
                {
                    this.Field[i, j] = null;
                }
            }
        }

        private bool CheckForVerticalWinner(Player player)
        {
            int count;
            Player? lastPlayer;

            // go throw columns
            for (int i = 0; i < this.Field.GetLength(0); i++)
            {
                count = 0;
                lastPlayer = null;

                // go throw rows
                for (int j = 0; j < this.Field.GetLength(1); j++)
                {
                    if (this.Field[i, j] != null && lastPlayer == this.Field[i, j])
                    {
                        count++;

                        if (count == 2)
                        {
                            return this.Field[i, j] == player;
                        }
                    }
                    else
                    {
                        count = 0;
                    }

                    lastPlayer = this.Field[i, j];
                }
            }

            return false;
        }

        private bool CheckForHorizontalWinner(Player player)
        {
            int count;
            Player? lastPlayer;

            // go throw rows
            for (int i = 0; i < this.Field.GetLength(1); i++)
            {
                count = 0;
                lastPlayer = null;

                // go throw columns
                for (int j = 0; j < this.Field.GetLength(0); j++)
                {
                    if (this.Field[j, i] != null && lastPlayer == this.Field[j, i])
                    {
                        count++;

                        if (count == 2)
                        {
                            return this.Field[j, i] == player;
                        }
                    }
                    else
                    {
                        count = 0;
                    }

                    lastPlayer = this.Field[j, i];
                }
            }

            return false;
        }

        private bool CheckForDiagonalWinner(Player player)
        {
            int count = 0;
            Player? lastPlayer = null;

            // go throw rows
            for (int i = 0; i < this.Field.GetLength(1); i++)
            {

                if (this.Field[i, i] != null && lastPlayer == this.Field[i, i])
                {
                    count++;

                    if (count == 2)
                    {
                        return this.Field[i, i] == player;
                    }
                }
                else
                {
                    count = 0;
                }

                lastPlayer = this.Field[i, i];

            }

            return false;
        }

        private bool CheckForDiagonalDownWinner(Player player)
        {
            int count = 0;
            Player? lastPlayer = null;
            int rowIndex = 2;

            for (int j = 0; j < this.Field.GetLength(0); j++)
            {
                if (this.Field[j, rowIndex] != null && lastPlayer == this.Field[j, rowIndex])
                {
                    count++;

                    if (count == 2)
                    {
                        return this.Field[j, rowIndex] == player;
                    }
                }
                else
                {
                    count = 0;
                }

                lastPlayer = this.Field[j, rowIndex];
                rowIndex--;

                if (rowIndex < 0)
                {
                    break;
                }
            }

            return false;
        }
    }
}
