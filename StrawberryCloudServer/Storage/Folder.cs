using System;
using System.IO;
using System.Linq;

namespace StrawberryCloudServer.Storage
{
    class Folder
    {
        private string rootPath;

        public Folder()
        {
            rootPath = Environment.GetEnvironmentVariable("rootPath", EnvironmentVariableTarget.User);
        }

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
            
            if (!directory.Exists)
            {
                directory.Create();
                return "null";
            }

            var list = directory.GetDirectories().ToList();

            return string.Join("/", list);
        }

        // 폴더 만들기
        public void SetFolder(string userId, string path, string folderName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath + "\\" + userId + path + "\\" + folderName);

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }

        // 폴더 삭제
        public void DeleteFolder(string userId, string path, string folderName)
        {
            DirectoryInfo directory = new DirectoryInfo(rootPath + "\\" + userId + path + "\\" + folderName);

            if(directory.Exists)
            {
                directory.Delete(true);
            }
        }
    }
}
