using StrawberryCloud.Commands;
using StrawberryCloud.Views.HomeContent;
using System.ComponentModel;
using System.Windows.Input;

namespace StrawberryCloud.ViewModels.HomeContent
{
    class AskNewFolderViewModel : INotifyPropertyChanged
    {
        public delegate void OnClose(string folderName);
        public event OnClose close;
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand confirmCommand { get; set; }
        private string FolderName = string.Empty;

        public string folderName
        {
            get { return FolderName; }
            set { FolderName = value; }
        }

        public AskNewFolderViewModel()
        {
            confirmCommand = new RelayCommand(confirmExeucteMethod);
        }

        private void confirmExeucteMethod(object obj)
        {
            close(folderName);
            (obj as AskNewFolderView).Close();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
