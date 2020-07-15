using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace StrawberryCloudServer.Storage
{
    class File
    {        
        private string rootPath;
        private Dictionary<string, string> status = new Dictionary<string, string>();

        public File()
        {
            rootPath = Environment.GetEnvironmentVariable("rootPath", EnvironmentVariableTarget.User);
        }

        // 이름들 가져오기
        public string GetNames(string userId, string addPathOrNull)
        {
            DirectoryInfo directory;

            if (addPathOrNull == "null")
            {
                directory = new DirectoryInfo(rootPath + "\\" + userId);
            }

            else
            {
                directory = new DirectoryInfo(rootPath + "\\" + userId + addPathOrNull);
            }

            var list = directory.GetFiles().ToList();

            return string.Join("/", list);
        }
        
        // 파일 다운로드
        public byte[] GetFile(string userId, string fileName, int index)
        {
            byte[] read = new byte[8192];

            using (FileStream fs = new FileStream(rootPath + "\\" + userId + "\\" + fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int readLen = 0;

                // 읽어야 하는 남은 용량이 바이트 크기보다 작은 경우를 위한 변수
                int readTemp = 0;
                fs.Position = index;

                while (readLen != read.Length)
                {
                    readTemp = readLen;
                    readLen += fs.Read(read, readLen, read.Length - readLen);

                    if(readTemp == readLen)
                    {
                        break;
                    }
                }                

                byte[] name = Encoding.UTF8.GetBytes(fileName);
                
                // 끝에 남는 쪼가리 용량을 위해 읽은만큼 복사한다
                byte[] copy = new byte[readLen];
                Array.Copy(read, copy, readLen);

                byte[] copyLen = BitConverter.GetBytes(readLen);
                byte[] send = new byte[copyLen.Length + copy.Length + name.Length];

                // 길이 먼저 보내준다
                copyLen.CopyTo(send, 0);
                copy.CopyTo(send, 4);
                name.CopyTo(send, 4 + copy.Length);

                return send;
            }                        
        }

        // 파일 업로드 전 존재유무 확인
        public string SetFileInfo(string userId, string fileName)
        {
            FileInfo file = new FileInfo(rootPath + userId + fileName);
            
            if(file.Exists)
            {
                file.Delete();
            }

            status.Add(file.Name, rootPath + userId + fileName);
            return file.Name;
        }

        public string SetFile(byte[] data)
        {
            int index = BitConverter.ToInt32(data, 0);
            data = data.Skip(4).ToArray();
            // data => byteLen, byte, fileName

            int byteLen = BitConverter.ToInt32(data, 0);
            string fileName = Encoding.UTF8.GetString(data, 4 + byteLen,  data.Length - byteLen - 4);

            data = data.Skip(4).ToArray();

            using (FileStream fs = new FileStream(status[fileName], FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                fs.Position = index;
                fs.Write(data, 0, byteLen);

                return fileName;
            }
        }

        public string SetFileEnd(string fileName)
        {
            status.Remove(fileName);

            return "null";
        }

        // 파일 이름, 크기 가져오기
        public string GetFileInfo(string userId, string fileName)
        {
            FileInfo file = new FileInfo(rootPath + "\\" + userId + fileName);

            object[] value = { file.Name, file.Length.ToString() };

            return string.Join("/", value);
        }

        // 파일들 사이즈 가져오기
        // addPath => null일 경우 최상위 폴더로 간주
        public string GetSizes(string userId, string addPathOrNull)
        {
            DirectoryInfo directory;


            if (addPathOrNull == "null")
            {
                directory = new DirectoryInfo(rootPath + "\\" + userId);
            }

            else
            {
                directory = new DirectoryInfo(rootPath + "\\" + userId + addPathOrNull);
            }

            var list = directory.GetFiles();
            List<string> size = new List<string>();

            foreach(FileInfo i in list)
            {
                size.Add(i.Length.ToString());
            }

            return string.Join("/", size);
        }

    }
}
