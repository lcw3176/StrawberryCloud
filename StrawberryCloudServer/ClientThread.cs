using StrawberryCloudServer.Enumerate;
using StrawberryCloudServer.Routes;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace StrawberryCloudServer
{
    class ClientThread
    {
        Socket socket;

        public ClientThread(Socket socket)
        {
            this.socket = socket;
            this.socket.ReceiveBufferSize = 0;
        }

        public void Run()
        {
            byte[] recv = new byte[4];
            Index index = new Index();
            Type type = index.GetType();
            Stopwatch sw = new Stopwatch();

            while (socket.Connected)
            {
                try
                {
                    sw.Start();
                    socket.Receive(recv, 0, recv.Length, SocketFlags.None);
                    int len = BitConverter.ToInt32(recv, 0);

                    byte[] dataRecv = new byte[len];

                    int recvLen = 0;

                    while (len > recvLen)
                    {
                        recvLen += socket.Receive(dataRecv, recvLen, len - recvLen, SocketFlags.None);
                    }


                    DataInfo dataInfo = (DataInfo)BitConverter.ToInt32(dataRecv, 0);
                    Method method = (Method)BitConverter.ToInt32(dataRecv, 4);
                    Destination destination = (Destination)BitConverter.ToInt32(dataRecv, 8);
                    dataRecv = dataRecv.Skip(12).ToArray();
                    object[] data = { method, dataRecv };

                    MethodInfo methodInfo = type.GetMethod(dataInfo.ToString());

                    byte[] viewModelToGo = BitConverter.GetBytes((int)destination);
                    byte[] result = (byte[])methodInfo.Invoke(index, data);
                    byte[] copyMethod = BitConverter.GetBytes((int)method);
                    byte[] send = new byte[viewModelToGo.Length + copyMethod.Length + result.Length];

                    viewModelToGo.CopyTo(send, 0);
                    copyMethod.CopyTo(send, 4);
                    result.CopyTo(send, 8);

                    socket.Send(BitConverter.GetBytes(send.Length), 0, 4, SocketFlags.None);
                    int sendLen = 0;

                    while(send.Length > sendLen)
                    {
                        sendLen += socket.Send(send, sendLen, send.Length - sendLen, SocketFlags.None);
                    }

                    sw.Stop();
                    Console.WriteLine(sw.ElapsedMilliseconds.ToString() + "ms");
                    sw.Reset();

                }

                catch(SocketException)
                {
                    Console.WriteLine(socket.RemoteEndPoint + " 연결 종료");
                    socket.Close();
                    break;
                }
              
            }


        }
    }
}
