using StrawberryCloud.Commands;
using StrawberryCloud.ViewModels.HomeContent;
using System.Collections.Generic;
using System.Windows.Input;

namespace StrawberryCloud.ViewModels
{
    class HomeViewModel : BaseViewModel
    {
        public ContentBaseViewModel contentViewModel { get; set; }
        public ICommand changeContentView { get; set; }
        private Dictionary<string, ContentBaseViewModel> contentDictionary = new Dictionary<string, ContentBaseViewModel>();

        public HomeViewModel()
        {
            ProfileViewModel profile = new ProfileViewModel();
            profile._changeViewToDownload += ChangeViewToDownload;
            DownloadViewModel download = new DownloadViewModel();

            contentDictionary.Add("Profile", profile);
            contentDictionary.Add("Download", download);
            contentViewModel = contentDictionary["Profile"];
            changeContentView = new RelayCommand(ChangeViewExecuteMethod);
        }

        // 다운르도 뷰로 넘길 때 정보 넘기기
        private void ChangeViewToDownload(object behavior, object path, object fileName, object dynamicPath)
        {
            if (behavior.ToString().Equals("Download"))
            {
                contentViewModel = contentDictionary["Download"];
                (contentViewModel as DownloadViewModel).SetDownloadPath(path, fileName, dynamicPath);
            }

            if(behavior.ToString().Equals("Upload"))
            {
                contentViewModel = contentDictionary["Download"];
                (contentViewModel as DownloadViewModel).SetUploadPath(path, fileName, dynamicPath);
            }

            OnPropertyChanged("contentViewModel");
        }

        // HomeView 컨텐츠 바꾸기
        private void ChangeViewExecuteMethod(object param)
        {
            if(param.ToString().Equals("Profile"))
            {
                contentViewModel = contentDictionary["Profile"];
                (contentViewModel as ProfileViewModel).RefreshExecuteMethod(null);
            }

            if(param.ToString().Equals("Download"))
            {
                contentViewModel = contentDictionary["Download"];
            }

            OnPropertyChanged("contentViewModel");
        }


    }
}
