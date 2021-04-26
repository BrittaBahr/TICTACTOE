using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using TicTacToeGameLogic;
using WpfClient.ViewModel.Event;

namespace WpfClient.ViewModel
{
    public class PlayerVM
    {
        private readonly Player model;

        public PlayerVM(Player model, AlphaRedGreenBlue argb)
        {
            this.model = model;
            this.ARGB = argb;
        }

        public string Name
        {
            get
            {
                return this.model.Nickname;
            }
        }

        public AlphaRedGreenBlue ARGB
        {
            get;
            set;
        }

        public bool CheckForModel(Player model)
        {
            return model == this.model;
        }
    }
}
