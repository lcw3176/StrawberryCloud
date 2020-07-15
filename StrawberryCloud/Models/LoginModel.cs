using StrawberryCloud.Models.Enumerate;
using StrawberryCloud.Models.Network;
using System.Text;

namespace StrawberryCloud.Models
{
    class LoginModel
    {
        public delegate void State();
        public event State success;
        public event State failed;

        private string userId;
        
        public string UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public LoginModel()
        {
            SocketConnection.GetInstance().GetConnection();
            SocketConnection.GetInstance().loginView += Receive;
        }

        private void Receive(Method method, byte[] data, int recvLen)
        {
            string result = Encoding.UTF8.GetString(data, 0, recvLen);

            if(result == "true")
            {
                Release();
                success();
            }

            else
            {
                failed();
            }
        }

        public void TryLogin(string userPw)
        {
            SocketConnection.GetInstance().Send(DataInfo.Login, Method.GET, Destination.LoginView, userId, userPw);
        }

        private void Release()
        {
            SocketConnection.GetInstance().loginView -= Receive;
        }
    }
}
