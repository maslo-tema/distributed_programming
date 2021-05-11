using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chain
{
    class Program
    {
        private static Socket _sender;
        private static Socket _listener;
        private static int _number; 

        static void Main(string[] args)
        {
            if (args.Length == 3 || args.Length == 4)
            {
                int listenPort = Convert.ToInt32(args[0]);
                string nextHost = args[1];
                int nextPort = Convert.ToInt32(args[2]);
                bool isFirst = false;

                if (args.Length == 4 && args[3] == "true")
                {
                    isFirst = true;
                }

                InitConnection(listenPort, nextHost, nextPort);

                _number = Convert.ToInt32(Console.ReadLine());

                if (isFirst)
                {
                    RunInitiator();
                }
                else
                {
                    RunNormalProcess();
                }

                _sender.Shutdown(SocketShutdown.Both);
                _sender.Close();
            }

            else
            {
                Console.WriteLine("Invalid argument count");
                Console.WriteLine("Use: dotnet run <listening-port> <next-host> <next-port> [true]");
            }
        }

        private static void InitConnection(int listenPort, string nextHost, int nextPort)
        {
            IPAddress listenIpAddress = IPAddress.Any;
            IPEndPoint localEP = new IPEndPoint(listenIpAddress, listenPort);
            _listener = new Socket(listenIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(localEP);
            _listener.Listen(10);
            
            IPAddress ipArdreess = nextHost == "localhost" ? IPAddress.Loopback : IPAddress.Parse(nextHost);
            IPEndPoint remoteEP = new IPEndPoint(ipArdreess, nextPort);
            _sender = new Socket(ipArdreess.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            Connecting(remoteEP);
        }

        private static void Connecting(IPEndPoint remoteEP)
        {
            try
            {
                _sender.Connect(remoteEP);
            }
            catch (SocketException)
            {
                Console.WriteLine("Failed to connecting");
            }
        }

        private static void RunInitiator()
        {
            _sender.Send(Encoding.UTF8.GetBytes("" + _number));
            Socket handler = _listener.Accept();
            
            int result =  GetNumber(handler);
            Console.WriteLine(result);

            int max = Math.Max(_number, result);
            _sender.Send(Encoding.UTF8.GetBytes("" + Math.Max(_number, max)));
            
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void RunNormalProcess()
        {
            Socket handler = _listener.Accept();
            int result = GetNumber(handler);

            int max = Math.Max(_number, result);
            _sender.Send(Encoding.UTF8.GetBytes("" + max));

            result = GetNumber(handler);
            Console.WriteLine(result);

            _sender.Send(Encoding.UTF8.GetBytes("" + result));

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static int GetNumber(Socket handler)
        {
            string temp = "";
            byte[] buffer = new byte[1024];
            int bytes;
            do
            {
                bytes = handler.Receive(buffer);
                temp += Encoding.UTF8.GetString(buffer, 0, bytes);
            }
            while (handler.Available > 0);

            int result = Convert.ToInt32(temp);
            return result;
        }
    }
}
