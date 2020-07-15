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
    }
}
