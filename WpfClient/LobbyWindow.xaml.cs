using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfClient.ViewModel;

namespace WpfClient
{
    /// <summary>
    /// Interaktionslogik für LobbyWindow.xaml
    /// </summary>
    public partial class LobbyWindow : Window
    {
        private MainWindow? mainWindow;

        private readonly NetworkAnnouncementClientServiceVM clientVM;

        private readonly IServiceProvider serviceProvider;

        public LobbyWindow(NetworkAnnouncementClientServiceVM clientVM, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.serviceProvider = serviceProvider;
            this.clientVM = clientVM;
            this.clientVM.GameStarted += ClientVMGameStarted;
            this.clientVM.PlayerRequestedGame += ClientVMPlayerRequestedGame;
            this.DataContext = this.clientVM;
        }

        private void ClientVMPlayerRequestedGame(object? sender, NwAnnouncementClientServicePlayerRequestedGameEventArgs e)
        {
            string text = $"Do you want to play with Player: {e.Player.Nickname}?";
            MessageBoxResult result = MessageBox.Show(text, "Attention", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                e.Player.SendAknGameRequest();
                Dispatcher.Invoke(() => 
                {
                    mainWindow = new MainWindow(this.serviceProvider, e.Player, this.clientVM.Nickname!, false);
                    mainWindow.Show();
                    this.Close();
                });
            }
        }

        private void ClientVMGameStarted(object? sender, NwAnnouncementClientServiceGameStartedEventArg e)
        {
            this.Dispatcher.Invoke(() =>
            {
                mainWindow = new MainWindow(this.serviceProvider, e.OtherPlayerVM, this.clientVM.Nickname!, true);
                mainWindow.Show();
                this.Close();
            });
        }
    }
}
