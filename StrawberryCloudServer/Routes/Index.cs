using StrawberryCloudServer.DataBase;
using StrawberryCloudServer.Enumerate;
using StrawberryCloudServer.Storage;
using System.Text;

namespace StrawberryCloudServer.Routes
{
    class Index
    {
        private string userId;
        private Folder folder;
        private Storage.File file;

        public Index()
        {
            folder = new Folder();
            file = new Storage.File();
        }

        public byte[] Login(Method method, byte[] _data)
        {
            DbRead db = new DbRead();
            string data = Encoding.UTF8.GetString(_data);
            string name = data.Split(',')[0];
            string password = data.Split(',')[1];
            byte[] send;

            if (db.IsUser(name, password))
            {
                send = Encoding.UTF8.GetBytes("true");
                this.userId = data.Split(',')[0];
                return send;
            }

            send = Encoding.UTF8.GetBytes("false");            
            return send;
        }

        public byte[] Init(Method method, byte[] _data)
        {
            byte[] send;
            StringBuilder sb = new StringBuilder();
            
            sb.Append(folder.GetNames(userId, "null"));

            if(sb.ToString() == "null")
            {
                send = Encoding.UTF8.GetBytes("null");
                return send;
            }

            sb.Append("&");            
            sb.Append(file.GetNames(userId, "null"));

            sb.Append("&");
            sb.Append(file.GetSizes(userId, null));
            send = Encoding.UTF8.GetBytes(sb.ToString());

            return send;
        }

        public byte[] Folder(Method method, byte[] _data)
        {
            string data = Encoding.UTF8.GetString(_data);
            byte[] send;
            StringBuilder sb = new StringBuilder();


            sb.Append(folder.GetNames(userId, data));

            if (sb.ToString() == "null")
            {
                send = Encoding.UTF8.GetBytes("null");
                return send;
            }

            sb.Append("&");
            sb.Append(file.GetNames(userId, data));

            sb.Append("&");
            sb.Append(file.GetSizes(userId, data));
            send = Encoding.UTF8.GetBytes(sb.ToString());

            return send;
        }

        public byte[] File(Method method, byte[] _data)
        {
            byte[] send = null;

            // 파일 다운로드 요청
            if (method.Equals(Method.GET))
            {
                string data = Encoding.UTF8.GetString(_data);
                string result = file.GetFileInfo(userId, data);

                send = Encoding.UTF8.GetBytes(result);
            }

            // 파일 업로드 요청
            if (method.Equals(Method.POST))
            {
                string filePath = Encoding.UTF8.GetString(_data);
                string fileName = file.SetFileInfo(userId, filePath);

                send = Encoding.UTF8.GetBytes(fileName);
            }

            // 파일 다운로드 시작
            if (method.Equals(Method.DOWNLOAD))
            {
                string data = Encoding.UTF8.GetString(_data);
                string fileName = data.Split(',')[0];
                int index = int.Parse(data.Split(',')[1]);

                send = file.GetFile(userId, fileName, index);
            }

            // 파일 업로드 시작
            if(method.Equals(Method.UPLOAD))
            {
                string result = file.SetFile(_data);

                send = Encoding.UTF8.GetBytes(result);
            }

            // 업로드 끝
            if(method.Equals(Method.UPLOADEND))
            {
                string data = Encoding.UTF8.GetString(_data);
                string result = file.SetFileEnd(data);

                send = Encoding.UTF8.GetBytes(result);
            }


            return send;
        }

    }
}
