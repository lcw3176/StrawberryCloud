using StrawberryCloud.Models.Network;
using System.ComponentModel;

namespace StrawberryCloud.ViewModels
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private BaseViewModel ViewModel;

        public void closeExecuteMethod(object sender, CancelEventArgs e)
        {
            SocketConnection.GetInstance().CloseSocket();
        }

        public void OnPropertyChanged(string param)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(param));
        }

        public void ChangeViewModel(BaseViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public BaseViewModel viewModel
        {
            get { return this.ViewModel; }
            set
            {
                this.ViewModel = value;
                OnPropertyChanged("viewModel");
            }
        }
    }
}
