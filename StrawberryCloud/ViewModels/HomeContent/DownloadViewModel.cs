using StrawberryCloud.Commands;
using StrawberryCloud.Models.HomeContent;
using StrawberryCloud.Models.HomeContent.ObservableCollection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StrawberryCloud.ViewModels.HomeContent
{
    class DownloadViewModel : ContentBaseViewModel
    {
        private DownloadModel model;
        public ICommand againDownloadCommand 
        {
            get { return model.AgainDownloadCommand; }
            set { model.AgainDownloadCommand = value; }
        }

        public ICommand pauseDownloadCommand
        {
            get { return model.PauseDownloadCommand; }
            set { model.PauseDownloadCommand = value; }
        }


        public ICommand deleteDownloadCommand 
        {
            get { return model.DeleteDownloadCommand; }
            set { model.DeleteDownloadCommand = value; }
        }

        public Dictionary<string, FileStatus> downloadStatus
        {
            get { return model.DownloadStatus; }
            set { model.DownloadStatus = value; }
        }

        public Dictionary<string, string> uploadPath
        {
            get { return model.UploadPath; }
            set { model.UploadPath = value; }
        }

        public Dictionary<string, string> downloadPath
        {
            get { return model.DownloadPath; }
            set { model.DownloadPath = value; }
        }

        public ObservableCollection<DownloadList> downloadList
        {
            get { return model.DownloadList; }
            set
            {
                model.DownloadList = value;
                OnPropertyChanged("downloadList");
            }
        }
        

        public DownloadViewModel()
        {
            model = new DownloadModel();
            againDownloadCommand = new RelayCommand(againDownloadExecuteMethod);
            pauseDownloadCommand = new RelayCommand(pauseDownloadExecuteMethod);
            deleteDownloadCommand = new RelayCommand(deleteDownloadExecuteMethod);
        }

        // 다운로드 목록 삭제(우클릭 메뉴)
        private void deleteDownloadExecuteMethod(object obj)
        {
            if(downloadStatus.ContainsKey(obj.ToString()))
            {
                if(MessageBox.Show("아직 진행중인 파일입니다. 계속하시겠습니까?", "경고", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }

            var component = downloadList.FirstOrDefault(e => e.name == obj.ToString());

            if (component != null)
            {
                downloadStatus.Remove(obj.ToString());
                downloadPath.Remove(obj.ToString());
                downloadList.Remove(component);
            }
        }

        // 다운로드 일시정지(우클릭 메뉴)
        private void pauseDownloadExecuteMethod(object obj)
        {
            var component = downloadStatus[obj.ToString()];

            if (component != null)
            {
                component.isRun = false;
            }
        }

        // 다운로드 다시 시작(우클릭 메뉴)
        private void againDownloadExecuteMethod(object obj)
        {
            var component = downloadStatus[obj.ToString()];

            if (component != null)
            {
                if(component.behavior == "Upload")
                {
                    model.RequestUpload(model.ReadFile(component.fileName), component.index);
                }

                if(component.behavior == "Download")
                {
                    model.RequestDownload(component.dynamicPath, component.index);
                }

                component.isRun = true;
            }
        }

        public void SetUploadPath(object path, object fileName, object dynamicPath)
        {
            if(uploadPath.ContainsKey(fileName.ToString()))
            {
                MessageBox.Show("이미 업로드 중입니다.");
                return;
            }

            uploadPath.Add(fileName.ToString(), path.ToString());
            model.TryUpload(dynamicPath.ToString(), fileName.ToString());
        }


        public void SetDownloadPath(object path, object fileName, object dynamicPath)
        {
            if(downloadPath.ContainsKey(fileName.ToString()))
            {
                MessageBox.Show("이미 다운로드 중입니다.");
                return;
            }

            if (File.Exists(path.ToString() + "\\" + fileName))
            {
                if(MessageBox.Show("이미 다운로드 한 이력이 있습니다. 새로 다운하시겠습니까?", "알람", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete(path.ToString() + "\\" + fileName);
                    downloadList.Remove(downloadList.FirstOrDefault(e => e.name == fileName.ToString()));
                }

                else
                {
                    return;
                }
            }

            downloadPath.Add(fileName.ToString(), path.ToString());
            model.TryDownload(dynamicPath.ToString(), fileName.ToString());
        }


    }
}
