using System;
using System.Collections.Generic;
using System.Text;

namespace WpfClient.ViewModel.Event
{
    public class NetworkPlayerGameStartedEventArgs : EventArgs
    {
        public NetworkPlayerGameStartedEventArgs(NetworkPlayerVM otherPlayerVM)
        {
            this.OtherPlayerVM = otherPlayerVM;
        }

        public NetworkPlayerVM OtherPlayerVM
        {
            get;
            private set;
        }
    }
}
