using StrawberryCloud.Commands;
using StrawberryCloud.Models.HomeContent;
using StrawberryCloud.Models.HomeContent.ObservableCollection;
using StrawberryCloud.Views.HomeContent;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace StrawberryCloud.ViewModels.HomeContent
{
    class ProfileViewModel : ContentBaseViewModel
    {
        private ProfileModel model;
        public ICommand fallbackCommand { get; set; }
        public ICommand uploadCommand { get; set; }
        public ICommand refreshCommand { get; set; }
        public ICommand makeFolderCommand { get; set; }

        public string dynamicPath
        {
            get { return model.DynamicPath; }
            set { model.DynamicPath = value; }
        }

        public ICommand deleteCommand
        {
            get { return model.DeleteCommand; }
            set { model.DeleteCommand = value; }
        }

        public ICommand clickCommand
        {
            get { return model.ClickCommand; }
            set { model.ClickCommand = value; }
        }

        public ObservableCollection<FileList> fileList
        {
            get { return model.FileList; }
            set
            {
                model.FileList = value;
                OnPropertyChanged("fileList");
            }
        }

        public ProfileViewModel()
        {
            model = new ProfileModel();
            clickCommand = new RelayCommand(ClickExecuteMethod);
            fallbackCommand = new RelayCommand(FallbackExecuteMethod);
            uploadCommand = new RelayCommand(UploadExecuteMethod);
            refreshCommand = new RelayCommand(RefreshExecuteMethod);
            deleteCommand = new RelayCommand(deleteExecuteMethod);
            makeFolderCommand = new RelayCommand(makeFolderExecuteMethod);
        }

        // 폴더 만들기 버튼
        private void makeFolderExecuteMethod(object obj)
        {
            AskNewFolderViewModel folderViewModel = new AskNewFolderViewModel();
            folderViewModel.close += model.MakeFolder;
            AskNewFolderView askNewFolder = new AskNewFolderView() 
            { 
                DataContext = folderViewModel 
            };
            askNewFolder.Show();

        }

        // 파일 삭제 버튼
        private void deleteExecuteMethod(object obj)
        {
            model.Delete(obj.ToString());
        }

        // 새로고침 버튼
        public void RefreshExecuteMethod(object obj)
        {
            model.Refresh();
        }

        // 파일 업로드 버튼
        private void UploadExecuteMethod(object obj)
        {
            Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();            
            
            if(open.ShowDialog().Value)
            {
                ChangeViewToDownload("Upload", open.FileName.Replace(open.SafeFileName, string.Empty), open.SafeFileName, dynamicPath);
                dynamicPath = string.Empty;
            }
        }

        // 뒤로가기
        private void FallbackExecuteMethod(object obj)
        {
            model.ShowFolder(null);
        }

        private void ClickExecuteMethod(object obj)
        {
            // 폴더면 열어보기
            if (!obj.ToString().Contains("."))
            {
                model.ShowFolder(obj.ToString());
                return;
            }

            // 파일이면 다운받기
            FolderBrowserDialog folder = new FolderBrowserDialog();
            
            if (folder.ShowDialog() == DialogResult.OK)
            {
                ChangeViewToDownload("Download", folder.SelectedPath, obj.ToString(), dynamicPath);
                dynamicPath = string.Empty;
            }
        }

    }
}
