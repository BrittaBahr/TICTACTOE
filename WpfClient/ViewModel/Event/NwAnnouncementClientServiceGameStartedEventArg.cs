using System;
using System.Collections.Generic;
using System.Text;

namespace WpfClient.ViewModel
{
    public class NwAnnouncementClientServiceGameStartedEventArg : EventArgs
    {
        public NwAnnouncementClientServiceGameStartedEventArg(NetworkPlayerVM otherPlayerVM)
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
