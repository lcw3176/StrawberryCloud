using StrawberryCloud.Models.Network;
using StrawberryCloud.ViewModels;
using System.Windows;

namespace StrawberryCloud
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            BaseViewModel viewModel = new LoginViewModel();
            this.DataContext = viewModel;
            Closing += viewModel.closeExecuteMethod;
        }
    }
}
