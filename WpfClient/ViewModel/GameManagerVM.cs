using Network;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TicTacToeGameLogic;
using WpfClient.ViewModel.Event;

namespace WpfClient.ViewModel
{
    public class GameManagerVM : INotifyPropertyChanged
    {
        private readonly NetworkPlayerVM networkPlayer;

        private readonly Player localPlayer;

        private readonly Player rival;

        private readonly IWriter<string> errorWriter;

        private PlayerVM? activePlayerVM;

        private PlayerVM? winner;

        private bool localPlayerIsOnTurn;

        private string? whoWins;

        private string? whosTurn;

        public GameManagerVM(NetworkPlayerVM networkPlayer, string nicknameLocalPlayer, bool localPlayerFirstPlayer)
        {
            this.errorWriter = new ErrorWriterView();
            this.networkPlayer = networkPlayer;
            this.networkPlayer.GameTurnReceived += this.NetworkPlayerGameTurnReceived;
            this.networkPlayer.GameTurnAcknowledgeReceived += this.NetworkPlayerGameTurnAcknowledgeReceived;

            this.localPlayer = new Player(3, 3, nicknameLocalPlayer);

            this.rival = new Player(3, 3, this.networkPlayer.Nickname);

            if (networkPlayer.Nickname == nicknameLocalPlayer)
            {
                nicknameLocalPlayer = "YOU";
            }

            if (localPlayerFirstPlayer)
            {
                this.PlayerOneVM = new PlayerVM(this.localPlayer, new AlphaRedGreenBlue(255, 255, 0, 0));
                this.PlayerTwoVM = new PlayerVM(this.rival, new AlphaRedGreenBlue(255, 0, 0, 255));
            }
            else
            {
                this.PlayerOneVM = new PlayerVM(this.rival, new AlphaRedGreenBlue(255, 255, 0, 0));
                this.PlayerTwoVM = new PlayerVM(this.localPlayer, new AlphaRedGreenBlue(255, 0, 0, 255));
            }

            this.ActivePlayerVM = this.PlayerOneVM;
            this.WhosTurn = this.PlayerOneVM.Name;
            this.LocalPlayerIsOnTurn = localPlayerFirstPlayer;
            this.FieldVM = new List<List<Cell>>();
            this.SetField();
            this.Winner = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler<GameManagerConnectionClosedEventArgs>? ConnectionClosed;

        public string? WhoWins
        {
            get
            {
                return this.whoWins;
            }

            private set
            {
                if (this.whoWins != value)
                {
                    this.whoWins = value;
                    this.OnPropertyChanged(nameof(this.WhoWins));
                }
            }
        }

        public string? WhosTurn
        {
            get
            {
                return this.whosTurn;
            }

            private set
            {
                if (this.whosTurn != value)
                {
                    this.whosTurn = value;
                    this.OnPropertyChanged(nameof(this.WhosTurn));
                }
            }
        }

        public bool LocalPlayerIsOnTurn
        {
            get
            {
                return this.localPlayerIsOnTurn;
            }

            private set
            {
                if (this.localPlayerIsOnTurn != value)
                {
                    this.localPlayerIsOnTurn = value;
                    this.OnPropertyChanged(nameof(this.LocalPlayerIsOnTurn));
                }
            }
        }

        public PlayerVM PlayerOneVM
        {
            get;
            private set;
        }

        public PlayerVM PlayerTwoVM
        {
            get;
            private set;
        }

        public PlayerVM ActivePlayerVM
        {
            get
            {
                return this.activePlayerVM!;
            }
            private set
            {
                if (this.activePlayerVM != value)
                {
                    this.activePlayerVM = value;
                    this.OnPropertyChanged(nameof(this.ActivePlayerVM));
                }
            }
        }

        public PlayerVM? Winner
        {
            get
            {
                return this.winner;
            }

            private set
            {
                if (this.winner != value)
                {
                    this.winner = value;
                    this.OnPropertyChanged(nameof(this.Winner));
                }
            }
        }

        public List<List<Cell>> FieldVM
        {
            get;
            private set;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnConnectionClosed()
        {
            this.ConnectionClosed?.Invoke(this, new GameManagerConnectionClosedEventArgs());
        }

        private ICommand GetCellCommand(int column, int row)
        {
            return new Command(async objAsync =>
            {
                if (column < 0 || column >= this.FieldVM.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(column));
                }
                else if (row < 0 || row >= this.FieldVM.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(column));
                }

                if (this.Winner != null)
                {
                    return;
                }

                if (this.LocalPlayerIsOnTurn)
                {
                    try
                    {
                        await this.networkPlayer.RequestGameTurnAsync(new Position(column, row));
                        this.localPlayer.MakeGameTurn(new Position(column, row));

                        Cell cell = this.FieldVM[column][row];

                        if (!cell.Active)
                        {
                            cell.ARGB = this.ActivePlayerVM.ARGB;
                            cell.Active = true;
                        }

                        this.ChangeActivePlayer();
                        this.ChangeWinner();
                    }
                    catch (RequestNotAcceptedException)
                    {
                        this.errorWriter.Write("Your turn wasn't acknowledged. Please send your turn again.");
                    }
                    catch (Exception)
                    {
                        this.OnConnectionClosed();
                    }
                }
                else
                {
                    Cell cell = this.FieldVM[column][row];
                    this.rival.MakeGameTurn(new Position(column, row));

                    if (!cell.Active)
                    {
                        cell.ARGB = this.ActivePlayerVM.ARGB;
                        cell.Active = true;
                    }

                    this.ChangeActivePlayer();
                    this.ChangeWinner();
                }
            });
        }

        private void ChangeActivePlayer()
        {
            if (this.ActivePlayerVM == this.PlayerOneVM)
            {
                this.ActivePlayerVM = this.PlayerTwoVM;
                this.WhosTurn = this.PlayerTwoVM.Name;
            }
            else
            {
                this.ActivePlayerVM = this.PlayerOneVM;
                this.WhosTurn = this.PlayerOneVM.Name;
            }
        }

        private void ChangeWinner()
        {
            Player winner;
            if (this.localPlayer.Won)
            {
                winner = this.localPlayer;
            }
            else if (this.rival.Won)
            {
                winner = this.rival;
            }
            else
            {
                return;
            }

            if (this.PlayerOneVM.CheckForModel(winner))
            {
                this.Winner = this.PlayerOneVM;
                this.WhoWins = this.PlayerOneVM.Name;
                this.WhosTurn = null;
                this.LocalPlayerIsOnTurn = false;
            }
            else if (PlayerTwoVM.CheckForModel(winner))
            {
                this.Winner = this.PlayerTwoVM;
                this.WhoWins = this.PlayerTwoVM.Name;
                this.WhosTurn = null;
                this.LocalPlayerIsOnTurn = false;
            }
        }

        private void SetField()
        {
            // go throw the columns
            for (int i = 0; i < this.localPlayer.Field.GetLength(0); i++)
            {
                List<Cell> cells = new List<Cell>();

                // go throw the rows
                for (int j = 0; j < this.localPlayer.Field.GetLength(1); j++)
                {
                    cells.Add(new Cell(GetCellCommand(i, j), new AlphaRedGreenBlue(0, 200, 200, 200), this.LocalPlayerIsOnTurn));
                }

                this.FieldVM.Add(cells);
            }
        }

        private void UpdateEnablePropertyOfCells(bool value)
        {
            // go throw the columns
            for (int i = 0; i < this.localPlayer.Field.GetLength(0); i++)
            {
                // go throw the rows
                for (int j = 0; j < this.localPlayer.Field.GetLength(1); j++)
                {
                    this.FieldVM[i][j].IsEnabled = value;
                }
            }
        }

        private void LocalPlayerGameTurnAcknowledgeReceived(object? sender, AcknowledgeGameTurnReceivedEventArgs e)
        {
            this.LocalPlayerIsOnTurn = true;
            this.UpdateEnablePropertyOfCells(true);
        }

        private void LocalPlayerGameTurnReceived(object? sender, GameTurnReceivedEventArgs e)
        {
            try
            {
                this.LocalPlayerIsOnTurn = true;
                this.UpdateEnablePropertyOfCells(true);
            }
            catch (Exception)
            {
                this.errorWriter.Write("This wasn't a valid move. Your competitor cheats!");
            }
        }

        private void NetworkPlayerGameTurnAcknowledgeReceived(object? sender, NetworkPlayerGameTurnAcknowledgeReceivedEventArgs e)
        {
            this.LocalPlayerIsOnTurn = false;
            this.UpdateEnablePropertyOfCells(false);
        }

        private void NetworkPlayerGameTurnReceived(object? sender, NetworkPlayerGameTurnReceivedEventArgs e)
        {
            if (!this.LocalPlayerIsOnTurn)
            {
                this.FieldVM[e.Position.XPosition][e.Position.YPosition].CellCommand.Execute(null);
                this.networkPlayer.AcknowledgeGameTurnRequest();
                this.LocalPlayerIsOnTurn = true;
                this.UpdateEnablePropertyOfCells(true);
            }
        }
    }
}
