using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TicTacToeGameLogic;
using WpfClient.ViewModel.Event;

namespace WpfClient.ViewModel
{
    public class LocalPlayerVM
    {
        private readonly Player model;

        public LocalPlayerVM(Player model)
        {
            this.model = model;
            this.model.GameTurnAcknowledgeReceived += this.ModelGameTurnAcknowledgeReceived;
            this.model.GameTurnReceived += this.ModelGameTurnReceived;
        }

        public event EventHandler<LocalPlayerGameTurnReceivedEventArgs>? GameTurnReceived;

        public event EventHandler<LocalPlayerGameTurnAcknowledgeReceviedEventArgs>? GameTurnAcknowledgeReceived;

        public string Nickname
        {
            get
            {
                return this.model.Nickname;
            }
        }

        protected virtual void OnGameTurnReceived(Position position)
        {
            this.GameTurnReceived?.Invoke(this, new LocalPlayerGameTurnReceivedEventArgs(position));
        }

        protected virtual void OnGameTurnAcknowledgeReceived()
        {
            this.GameTurnAcknowledgeReceived?.Invoke(this, new LocalPlayerGameTurnAcknowledgeReceviedEventArgs());
        }

        private void ModelGameTurnReceived(object? sender, GameTurnReceivedEventArgs e)
        {
            this.OnGameTurnReceived(e.Position);
        }

        private void ModelGameTurnAcknowledgeReceived(object? sender, AcknowledgeGameTurnReceivedEventArgs e)
        {
            this.OnGameTurnAcknowledgeReceived();
        }
    }
}
