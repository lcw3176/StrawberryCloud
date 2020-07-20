using StrawberryCloud.Models.Enumerate;
using StrawberryCloud.Models.HomeContent.ObservableCollection;
using StrawberryCloud.Models.Network;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace StrawberryCloud.Models.HomeContent
{
    class ProfileModel
    {
        private ObservableCollection<FileList> fileList = new ObservableCollection<FileList>();
        private ICommand clickCommand;
        private ICommand deleteCommand;

        private string dynamicPath { get; set; }

        public string DynamicPath
        {
            get { return dynamicPath; }
            set { dynamicPath = value; }
        }

        public ICommand DeleteCommand
        {
            get { return deleteCommand; }
            set { deleteCommand = value; }
        }

        public ICommand ClickCommand
        {
            get { return clickCommand; }
            set { clickCommand = value; }
        }


        public ObservableCollection<FileList> FileList
        {
            get { return fileList; }
            set { fileList = value; }
        }

        public ProfileModel()
        {
            SocketConnection.GetInstance().profileView += Receive;
            Refresh();
        }

        public void Refresh()
        {
            SocketConnection.GetInstance().Send(DataInfo.Init, Method.GET, Destination.ProfileView, "null");
            dynamicPath = string.Empty;
        }

        private void Release()
        {
            SocketConnection.GetInstance().profileView -= Receive;
        }

        private void Receive(Method method, byte[] data, int recvLen)
        {
            // Destination, Method 두 개의 integer를 제외한 부분부터 읽는다 

            if (method.Equals(Method.GET))
            {
                string result = Encoding.UTF8.GetString(data, 8, recvLen);
                // 클라우드에 업로드한 파일이 없거나 빈 폴더를 열었을 경우
                if (result == "null")
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        FileList.Clear();
                    });
                }

                // 클라우드에 업로드한 파일이 없을 경우 패스
                if (result != "null")
                {
                    string folder = result.Split('&')[0];
                    string[] file = result.Split('&')[1].Split('/');
                    string[] fileSize = result.Split('&')[2].Split('/');

                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        FileList.Clear();

                        // 공백시 첫 한칸 추가 방지
                        if (!string.IsNullOrEmpty(folder.Split('/')[0]))
                        {
                            foreach (string i in folder.Split('/'))
                            {
                                FileList.Add(new FileList
                                {
                                    name = i,
                                    size = null,
                                    type = "Folder",
                                    clickCommand = ClickCommand,
                                    deleteCommand = DeleteCommand,
                                });
                            }
                        }


                        // 공백시 첫 한칸 추가 방지
                        if (!string.IsNullOrEmpty(file[0]))
                        {
                            for (int i = 0; i < file.Length; i++)
                            {
                                FileList.Add(new FileList
                                {
                                    name = file[i],
                                    size = TransferFileSize(double.Parse(fileSize[i])),
                                    type = "File",
                                    clickCommand = ClickCommand,
                                    deleteCommand = DeleteCommand,
                                });
                            }
                        }

                    });

                }
            }

            if (method.Equals(Method.DELETE))
            {
                string result = Encoding.UTF8.GetString(data, 8, recvLen);

                if(result.Equals("true"))
                {
                    Refresh();
                }
            }

            if(method.Equals(Method.UPDATE))
            {
                Refresh();
            }
           
        }

        // 사람이 읽기 좋은 표기법으로 파일 용량 변환
        private string TransferFileSize(double byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }

        public void ShowFolder(string folderName)
        {
            // 뒤로가기 커맨드
            if (folderName == null)
            {
                if (this.dynamicPath.Length == 0) { return; }
                int lastIndex = dynamicPath.LastIndexOf("\\");
                this.dynamicPath = this.dynamicPath.Remove(lastIndex, dynamicPath.Length - lastIndex);
            }
            
            else
            {
                this.dynamicPath += "\\" + folderName;
            }

            SocketConnection.GetInstance().Send(DataInfo.Folder, Method.GET, Destination.ProfileView, dynamicPath);

        }

        // 폴더 만들기
        public void MakeFolder(string folderName)
        {
            if (String.IsNullOrEmpty(folderName)) { return; }
            SocketConnection.GetInstance().Send(DataInfo.Folder, Method.UPDATE, Destination.ProfileView, dynamicPath, folderName);
        }

        // 파일 삭제
        public void Delete(string fileName)
        {
            var element = FileList.FirstOrDefault(e => e.name == fileName);

            if(element.type.Equals("File"))
            {
                SocketConnection.GetInstance().Send(DataInfo.File, Method.DELETE, Destination.ProfileView, dynamicPath, fileName);
            }

            if(element.type.Equals("Folder"))
            {
                SocketConnection.GetInstance().Send(DataInfo.Folder, Method.DELETE, Destination.ProfileView, dynamicPath, fileName);
            }

            fileList.Remove(element);

        }


    }
}
