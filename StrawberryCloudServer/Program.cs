using StrawberryCloudServer.DataBase;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace StrawberryCloudServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 3000));

            socket.Listen(1);

            while(true)
            {
                Socket user = socket.Accept();
                ClientThread clientThread = new ClientThread(user);
                Task.Run(() => clientThread.Run());
                Console.WriteLine("유저 접속: " + user.RemoteEndPoint);
            }
        }
    }
}
