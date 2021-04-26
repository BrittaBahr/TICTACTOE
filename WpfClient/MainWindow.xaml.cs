using Microsoft.Extensions.DependencyInjection;
using Network;
using SharedLibrary;
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
using TicTacToeGameLogic;
using WpfClient.ViewModel;

namespace WpfClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameManagerVM game;

        private readonly IServiceProvider serviceProvider;

        public MainWindow(IServiceProvider serviceProvider, NetworkPlayerVM networkPlayerVM, string nicknameLocalPlayer, bool localPlayerIsPlayerOne)
        {
            this.serviceProvider = serviceProvider;
            this.game = new GameManagerVM(networkPlayerVM, nicknameLocalPlayer, localPlayerIsPlayerOne);
            this.game.ConnectionClosed += this.GameConnectionClosed;
            this.DataContext = this.game;
            InitializeComponent();
        }

        private void GameConnectionClosed(object? sender, ViewModel.Event.GameManagerConnectionClosedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("The connection got lost.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            if (result == MessageBoxResult.OK)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                    Environment.Exit(1);
                });
            }
        }
    }
}
