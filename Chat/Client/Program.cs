﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    class Program
    {
        public static void StartClient(string address, int port, string text)
        {
            try
            {
                // Разрешение сетевых имён
                IPAddress ipAddress = address == "localhost" ? IPAddress.Loopback : IPAddress.Parse(address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // CREATE
                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                try
                {
                    // CONNECT
                    sender.Connect(remoteEP);

                    // SEND
                    int bytesSent = sender.Send(Encoding.UTF8.GetBytes(text));

                    // RECEIVE
                    byte[] buf = new byte[1024];
                    StringBuilder dataBuilder = new StringBuilder();
                    do
                    {
                        int bytesRec = sender.Receive(buf, buf.Length, 0);
                        dataBuilder.Append(Encoding.UTF8.GetString(buf, 0, bytesRec));
                    }
                    while (sender.Available > 0);

                    List<string> history = JsonSerializer.Deserialize<List<string>>(dataBuilder.ToString());

                    foreach (var message in history)
                    {
                        Console.WriteLine(message);
                    }

                    // RELEASE
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                StartClient(args[0], Int32.Parse(args[1]), args[2]);
            }
        }
    }
}
