using System.ComponentModel;
using System.Windows.Input;

namespace StrawberryCloud.Models.HomeContent.ObservableCollection
{
    class DownloadList : INotifyPropertyChanged
    {
        private double progressState;
        private string speedState;

        public string name { get; set; }
        public string size { get; set; }
        public string speed 
        {
            get { return speedState; }
            set
            {
                speedState = value;
                OnPropertyUpdate("speed");
            } 
        }
        public double progress
        {
            get { return progressState; }
            set
            {
                progressState = value;
                OnPropertyUpdate("progress");
            }
        }
        public string behavior { get; set; }

        public ICommand againDownloadCommand { get; set; }
        public ICommand pauseDownloadCommand { get; set; }
        public ICommand deleteDownloadCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyUpdate(string param)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(param));
            }
        }
    }
}
