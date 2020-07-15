using StrawberryCloud.Models.Enumerate;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrawberryCloud.Models.Network
{
    class SocketConnection
    {
        private static SocketConnection instance;
        private static Socket socket;

        public delegate void Receive(Method method, byte[] data, int recvLength);
        public event Receive loginView;
        public event Receive profileView;
        public event Receive downloadView;

        public static SocketConnection GetInstance()
        {
            if(instance == null)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveBufferSize = 0;
                instance = new SocketConnection();
            }

            return instance;
        }

        public void GetConnection()
        {
            if(!socket.Connected)
            {
                socket.Connect(new IPEndPoint(IPAddress.Loopback, 3000));
                Thread t = new Thread(StartReceive);
                t.Start();
            }
        }

        public void CloseSocket()
        {
            if (socket.Connected)
            {
                socket.Close();
            }
        }

        private void StartReceive()
        {
            byte[] byteLen = new byte[4];
            byte[] recv = new byte[1024];

            while (socket.Connected)
            {
                try
                {
                    socket.Receive(byteLen, 0, 4, SocketFlags.None);
                    int dataLen = BitConverter.ToInt32(byteLen, 0);
                    
                    if(recv.Length < dataLen)
                    {
                        recv = new byte[dataLen * 2];
                    }
                    
                    int recvLen = 0;

                    while (dataLen > recvLen)
                    {
                        recvLen += socket.Receive(recv, recvLen, dataLen - recvLen, SocketFlags.None);
                    }                    

                    Destination destination = (Destination)BitConverter.ToInt32(recv, 0);
                    Method method = (Method)BitConverter.ToInt32(recv, 4);

                    // 사이즈 조절, 필요없는 데이터 가공 후 넘겨주기
                    recv = recv.Skip(8).ToArray();

                    switch (destination)
                    {
                        case Destination.LoginView:
                            loginView(method, recv, dataLen - 8);
                            break;
                        case Destination.ProfileView:
                            profileView(method, recv, dataLen - 8);
                            break;
                        case Destination.DownloadView:
                            downloadView(method, recv, dataLen - 8);
                            break;
                        default:
                            break;
                    }
                }

                catch(Exception)
                {
                    continue;
                }

            }
        }

        public void Send(DataInfo _dataInfo, Method _method, Destination _destination, params string[] data)
        {
            StringBuilder sb = new StringBuilder();

            for(int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i]);
                sb.Append(",");
            }

            sb.Remove(sb.Length - 1, 1);

            byte[] info = BitConverter.GetBytes((int)_dataInfo);
            byte[] method = BitConverter.GetBytes((int)_method);
            byte[] destination = BitConverter.GetBytes((int)_destination);

            byte[] strData = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] send = new byte[info.Length + method.Length + +destination.Length + strData.Length];

            info.CopyTo(send, 0);
            method.CopyTo(send, 4);
            destination.CopyTo(send, 8);
            strData.CopyTo(send, 12);

            // 길이 먼저 보낸후 데이터를 보냄
            socket.Send(BitConverter.GetBytes(send.Length), 0, 4, SocketFlags.None);

            int sendLen = 0;

            while (send.Length > sendLen)
            {
                sendLen += socket.Send(send, sendLen, send.Length - sendLen, SocketFlags.None);
            }
            
        }

        public void Send(DataInfo _dataInfo, Method _method, Destination _destination, byte[] file, int _index)
        {
            byte[] info = BitConverter.GetBytes((int)_dataInfo);
            byte[] method = BitConverter.GetBytes((int)_method);
            byte[] destination = BitConverter.GetBytes((int)_destination);

            byte[] index = BitConverter.GetBytes(_index);
            byte[] send = new byte[info.Length + method.Length + destination.Length + file.Length + index.Length];

            info.CopyTo(send, 0);
            method.CopyTo(send, 4);
            destination.CopyTo(send, 8);
            index.CopyTo(send, 12);
            file.CopyTo(send, 16);
            

            // 길이 먼저 보낸후 데이터를 보냄
            socket.Send(BitConverter.GetBytes(send.Length), 0, 4, SocketFlags.None);

            int sendLen = 0;

            while (send.Length > sendLen)
            {
                sendLen += socket.Send(send, sendLen, send.Length - sendLen, SocketFlags.None);
            }

        }
    }
}
