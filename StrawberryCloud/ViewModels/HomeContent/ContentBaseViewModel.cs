using System.ComponentModel;

namespace StrawberryCloud.ViewModels.HomeContent
{
    class ContentBaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void changeViewToDownload(object behavior, object path, object fileName, object dynamicPath);
        public event changeViewToDownload _changeViewToDownload;


        public void ChangeViewToDownload(object behavior, object path, object fileName, object dynamicPath)
        {
            _changeViewToDownload(behavior, path, fileName, dynamicPath);
        }

        public void OnPropertyChanged(string param)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(param));
        }
    }
}
