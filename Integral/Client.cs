using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonNet;

namespace Integral
{
    class Client
    {
        public delegate void Log(string data);
        public event Log EventInfo;

        private string sourceData;
        private String serverHost;
        private Socket cSocket;
        private int port = 8034;
        private NetMessaging net;
        public Client(String serverHost)
        {
            try
            {
                this.serverHost = serverHost;
                Console.WriteLine("Подключение к {0}", this.serverHost);
                cSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                cSocket.Connect(this.serverHost, port);
                net = new NetMessaging(cSocket);
                net.DataReceived += OnResult;
                //подписки
                new Thread(() =>
                {
                    try
                    {
                        net.Communicate();
                    }
                    catch (Exception ex)
                    {
                        //exception
                    }
                }).Start();
            }
            catch (Exception e)
            {
                //exception
            }
        }
        public void Send(string data)
        {
            if(data.Trim().Length > 0)
            {
                sourceData = data;
                net.SendData("REQUEST",data);
            }
        }
        private void OnResult(string command,string data)
        {
            if (data.Trim().Equals("NOTHAVE"))
            {
                Integrate(sourceData);
            }
            else
            {
                EventInfo?.Invoke(data);
            }
        }
        private void Integrate(string data)
        {
            //метод интегрирования в пределех data(сплитить)
        }
    }
}
