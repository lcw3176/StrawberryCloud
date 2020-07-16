using StrawberryCloud.Models.Enumerate;
using StrawberryCloud.Models.HomeContent.ObservableCollection;
using StrawberryCloud.Models.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace StrawberryCloud.Models.HomeContent
{
    class DownloadModel
    {
        private ObservableCollection<DownloadList> downloadList = new ObservableCollection<DownloadList>();
        private Dictionary<string, string> downloadPath = new Dictionary<string, string>();
        private Dictionary<string, string> uploadPath = new Dictionary<string, string>();
        private Dictionary<string, FileStatus> downloadStatus = new Dictionary<string, FileStatus>();
        private ICommand againDownloadCommand;
        private ICommand deleteDownloadCommand;
        private ICommand pauseDownloadCommand;

        public ICommand AgainDownloadCommand
        {
            get { return againDownloadCommand; }
            set { againDownloadCommand = value; }
        }

        public ICommand DeleteDownloadCommand
        {
            get { return deleteDownloadCommand; }
            set { deleteDownloadCommand = value; }
        }

        public ICommand PauseDownloadCommand
        {
            get { return pauseDownloadCommand; }
            set { pauseDownloadCommand = value; }
        }

        public Dictionary<string, FileStatus> DownloadStatus
        {
            get { return downloadStatus; }
            set { downloadStatus = value; }
        }

        public Dictionary<string, string> UploadPath
        {
            get { return uploadPath; }
            set { uploadPath = value; }
        }

        public Dictionary<string, string> DownloadPath
        {
            get { return downloadPath; }
            set { downloadPath = value; }
        }

        public ObservableCollection<DownloadList> DownloadList
        {
            get { return downloadList; }
            set { downloadList = value; }
        }

        public DownloadModel()
        {
            SocketConnection.GetInstance().downloadView += Receive;
        }

        private void Release()
        {
            SocketConnection.GetInstance().downloadView -= Receive;
        }

        private void Receive(Method method, byte[] data, int recvLen)
        {
            // 다운로드 관련 코드 시작
            if(method.Equals(Method.GET))
            {
                string result = Encoding.UTF8.GetString(data, 0, recvLen);
                string dynamicPath = result.Split('/')[0];
                string fileName = result.Split('/')[1];
                string fileSize = result.Split('/')[2];

                downloadStatus.Add(fileName, new FileStatus()
                {
                    fileName = fileName,
                    size = double.Parse(fileSize),
                    index = 0,
                    isRun = true,
                    behavior = "Download",
                    dynamicPath = dynamicPath,
                });

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var isExist = DownloadList.FirstOrDefault(e => e.name == fileName && e.behavior == "Download");
                    
                    if(isExist != null)
                    {
                        DownloadList.Remove(isExist);
                    }

                    DownloadList.Add(new DownloadList()
                    {
                        name = fileName,
                        size = TransferFileSize(double.Parse(fileSize)),
                        speed = "0",
                        progress = 0,
                        behavior = "Download",
                        againDownloadCommand = AgainDownloadCommand,
                        deleteDownloadCommand = DeleteDownloadCommand,
                        pauseDownloadCommand = PauseDownloadCommand,
                    });

                });

                RequestDownload(dynamicPath, 0);
            }

            if (method.Equals(Method.DOWNLOAD))
            {
                int byteLen = BitConverter.ToInt32(data, 0);
                int fileNameLen = recvLen - byteLen;
                string fileName = Encoding.UTF8.GetString(data, byteLen + 4, fileNameLen - 4);

                var fileStatus = downloadStatus[fileName];

                data = data.Skip(4).ToArray();

                using (FileStream fs = new FileStream(downloadPath[fileName] + "\\" + fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    fs.Position = fileStatus.index;
                    fs.Write(data, 0, byteLen);
                }

                fileStatus.index += byteLen;
                var entity = downloadList.FirstOrDefault(e => e.name == fileName && e.behavior == "Download");

                // 유저가 임의로 목록 삭제했을 때
                if (entity == null) 
                {
                    return; 
                }

                entity.progress = (fileStatus.index / fileStatus.size) * 100;
                entity.speed = TransferFileSize(fileStatus.index);


                if (fileStatus.size > fileStatus.index)
                {
                    // 일시정지 눌렀을때 리턴
                    if (!fileStatus.isRun) { return; }
                    RequestDownload(fileStatus.dynamicPath, fileStatus.index);
                }

                // 다운 완료 시 정리
                else
                {
                    var classEntity = downloadList.FirstOrDefault(e => e.name == fileName && e.behavior == "Download");
                    classEntity.progress = 100;
                    classEntity.speed = entity.size;
                    downloadPath.Remove(fileStatus.fileName);
                    downloadStatus.Remove(fileName);
                }
            }
            // 다운로드 관련 코드 끝

            // 업로드 관련 코드 시작
            if(method.Equals(Method.POST))
            {
                string fileName = Encoding.UTF8.GetString(data, 0, recvLen);
                
                FileInfo file = new FileInfo(uploadPath[fileName] + fileName);

                downloadStatus.Add(fileName, new FileStatus()
                {
                    fileName = fileName,
                    size = file.Length,
                    index = 0,
                    isRun = true,
                    behavior = "Upload",
                });

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var isExist = DownloadList.FirstOrDefault(e => e.name == fileName && e.behavior == "Upload");

                    if (isExist != null)
                    {
                        DownloadList.Remove(isExist);
                    }

                    DownloadList.Add(new DownloadList()
                    {
                        name = fileName,
                        size = TransferFileSize(file.Length),
                        speed = "0",
                        progress = 0,
                        behavior = "Upload",
                        againDownloadCommand = AgainDownloadCommand,
                        deleteDownloadCommand = DeleteDownloadCommand,
                        pauseDownloadCommand = PauseDownloadCommand,
                    });

                });

                byte[] read = ReadFile(fileName);
                RequestUpload(read, 0);
            }

            if(method.Equals(Method.UPLOAD))
            {
                string fileName = Encoding.UTF8.GetString(data, 0, recvLen);
                var fileStatus = downloadStatus[fileName];               
                var entity = downloadList.FirstOrDefault(e => e.name == fileName && e.behavior == "Upload");

                // 유저가 임의로 목록 삭제했을 때
                if(entity == null) 
                {
                    downloadStatus.Remove(fileName);
                    downloadPath.Remove(fileName);
                    return; 
                }

                entity.progress = ((fileStatus.index) / fileStatus.size) * 100;
                entity.speed = TransferFileSize(fileStatus.index);


                if (fileStatus.size > fileStatus.index)
                {
                    int index = downloadStatus[fileName].index;
                    byte[] read = ReadFile(fileName);

                    // 일시정지 눌렀을때 리턴
                    if (!fileStatus.isRun) { return; }
                    RequestUpload(read, index);
                }

                // 업로드 완료 시 정리
                else
                {
                    var classEntity = downloadList.FirstOrDefault(e => e.name == fileName && e.behavior == "Upload");
                    classEntity.progress = 100;
                    classEntity.speed = entity.size;
                    uploadPath.Remove(fileStatus.fileName);
                    downloadStatus.Remove(fileName);

                    // 업로드 끝 알려주기
                    NoticeUploadEnd(fileName);                    
                }
               
            }

            // 서버에서도 업로드 종료
            if(method.Equals(Method.UPLOADEND))
            {
                // 리프레시 요청 코드
                SocketConnection.GetInstance().Send(DataInfo.Folder, Method.GET, Destination.ProfileView, "null");
            }

            // 업로드 관련 코드 끝
        }

        // 파일 읽기
        public byte[] ReadFile(string fileName)
        {
            var findFile = downloadStatus[fileName];
            byte[] read = new byte[8192];

            using (FileStream fs = new FileStream(uploadPath[fileName] + fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int readLen = 0;

                // 읽어야 하는 남은 용량이 바이트 크기보다 작은 경우를 위한 변수
                int readTemp = 0;
                fs.Position = findFile.index;

                while (readLen != read.Length)
                {
                    readTemp = readLen;
                    readLen += fs.Read(read, readLen, read.Length - readLen);

                    if (readTemp == readLen)
                    {
                        break;
                    }
                }

                findFile.index += readLen;
                byte[] copy = new byte[readLen];
                Array.Copy(read, copy, readLen);

                byte[] copyLen = BitConverter.GetBytes(readLen);
                byte[] name = Encoding.UTF8.GetBytes(fileName);

                byte[] send = new byte[copy.Length + copyLen.Length + name.Length];

                copyLen.CopyTo(send, 0);
                copy.CopyTo(send, 4);
                name.CopyTo(send, 4 + copy.Length);

                return send;
            }
        }

        // 서버로 요청 보내기, 준비시키기
        public void TryDownload(string dynamicPath, string fileName)
        {
            SocketConnection.GetInstance().Send(DataInfo.File, Method.GET, Destination.DownloadView, dynamicPath + "\\" + fileName);
        }

        // 서버로 요청 보내기, 준비시키기
        public void TryUpload(string dynamicPath, string fileName)
        {
            SocketConnection.GetInstance().Send(DataInfo.File, Method.POST, Destination.DownloadView, dynamicPath + "\\" + fileName);
        }

        // 다운로드 시작
        public void RequestDownload(string dynamicPath, int index)
        {
            SocketConnection.GetInstance().Send(DataInfo.File, Method.DOWNLOAD, Destination.DownloadView, dynamicPath, index.ToString());
        }

        // 업로드 시작
        public void RequestUpload(byte[] byteFile, int index)
        {
            SocketConnection.GetInstance().Send(DataInfo.File, Method.UPLOAD, Destination.DownloadView, byteFile, index);
        }

        // 업로드 종료 통보
        private void NoticeUploadEnd(string fileName)
        {
            SocketConnection.GetInstance().Send(DataInfo.File, Method.UPLOADEND, Destination.DownloadView, fileName);
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

    }
}
