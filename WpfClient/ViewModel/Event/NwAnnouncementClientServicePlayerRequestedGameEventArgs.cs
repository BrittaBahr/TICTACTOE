using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WpfClient.ViewModel
{
    public class NwAnnouncementClientServicePlayerRequestedGameEventArgs : EventArgs
    {
        public NwAnnouncementClientServicePlayerRequestedGameEventArgs(NetworkPlayerVM player)
        {
            this.Player = player;
        }

        public NetworkPlayerVM Player
        {
            get;
            private set;
        }
    }
}
