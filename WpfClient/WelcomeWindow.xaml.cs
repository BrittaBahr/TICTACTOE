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
using WpfClient.ViewModel;

namespace WpfClient
{
    /// <summary>
    /// Interaktionslogik für WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        private LobbyWindow? lobbyWindow;

        private readonly NetworkAnnouncementClientServiceVM clientVM;

        private readonly IServiceProvider serviceProvider;

        public WelcomeWindow()
        {
            this.InitializeComponent();
            this.serviceProvider = this.MakeServiceProvider();
            IAnnouncementClientService model = serviceProvider.GetRequiredService<IAnnouncementClientService>();
            this.clientVM = new NetworkAnnouncementClientServiceVM(model);
            this.clientVM.ValidNicknameReceived += ClientVMValidNicknameReceived;
            this.DataContext = this.clientVM;
        }

        private void ClientVMValidNicknameReceived(object? sender, ValidNicknameReceivedEventArgs e)
        {
            this.lobbyWindow = new LobbyWindow(this.clientVM, this.serviceProvider);
            this.lobbyWindow.Show();
            this.Close(); ;
        }

        private IServiceProvider MakeServiceProvider()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddTransient<IAnnouncementClientService>(x =>
                new NetworkAnnouncementClientService(12345));
            return serviceCollection.BuildServiceProvider();
        }
    }
}
